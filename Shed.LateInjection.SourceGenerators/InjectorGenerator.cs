using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Shed.LateInjection.SourceGenerators.Models;

namespace Shed.LateInjection.SourceGenerators;

[Generator]
public class InjectorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var lateInjectSymbols = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                "Shed.LateInjection.Attributes.LateInjectAttribute",
                static (node, _) =>
                    node is PropertyDeclarationSyntax
                    || node is VariableDeclaratorSyntax
                    || node is MethodDeclarationSyntax,
                static (node, _) => node.TargetSymbol);

        var memberInfosForInject = lateInjectSymbols
            .Combine(context.CompilationProvider)
            .Where(static tuple => SymbolEqualityComparer.Default.Equals(
                tuple.Left.ContainingAssembly, 
                tuple.Right.Assembly))
            .Select(static (tuple, _) => GetMemberInfo(tuple.Left));

        var typeInfosForInject = memberInfosForInject
            .Collect()
            .SelectMany(
                static (members, _) => members
                    .GroupBy(static member => member.ContainingType, SymbolEqualityComparer.Default)
                    .ToImmutableArray())
            .Select(
                static (group, _) => new InjectTypeInfo(
                    (ITypeSymbol)group.Key!,
                    group
                        .OrderBy(member => member is InjectFieldOrPropertyInfo ? 0 : 1)
                        .ToImmutableArray()));

        context.RegisterSourceOutput(
            typeInfosForInject.Collect(),
            GenerateInjector);

        var memberInfosForValidation = lateInjectSymbols
            .Select(static (symbol, _) => GetMemberInfo(symbol));

        context.RegisterSourceOutput(
            memberInfosForValidation.Collect(),
            GenerateValidator);
    }

    private static void GenerateValidator(
        SourceProductionContext context,
        ImmutableArray<InjectMemberInfo> memberInfos)
    {
        if (memberInfos.Length == 0)
            return;

        var source = ValidatorSourceBuilder.BuildLateInjectValidatorSource(memberInfos);

        context.AddSource(
            "LateInjectValidator.g.cs",
            SourceText.From(
                source,
                Encoding.UTF8));
    }

    private static void GenerateInjector(
        SourceProductionContext context,
        ImmutableArray<InjectTypeInfo> typeInfos)
    {
        if (typeInfos.Length == 0)
            return;

        var source = LateInjectionSourceBuilder.BuildLateInjectionSource(typeInfos);

        context.AddSource(
            "LateInjections.g.cs",
            SourceText.From(
                source,
                Encoding.UTF8));
    }

    private static InjectMemberInfo GetMemberInfo(ISymbol symbol)
    {
        if (symbol is IPropertySymbol property)
            return new InjectFieldOrPropertyInfo(property);

        if (symbol is IFieldSymbol field)
            return new InjectFieldOrPropertyInfo(field);

        if (symbol is IMethodSymbol method)
            return new InjectMethodInfo(method);

        throw new NotImplementedException();
    }
}
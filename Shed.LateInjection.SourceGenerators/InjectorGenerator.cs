using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Shed.LateInjection.SourceGenerators;

[Generator]
public class InjectorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var lateInjectMethods = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                "Shed.LateInjection.Attributes.LateInjectAttribute",
                NodeIsMethod,
                TryGetMethodInfo)
            .Where(static info => info is not null)
            .Select(static (info, _) => info!);

        context.RegisterSourceOutput(
            lateInjectMethods.Collect(),
            GenerateInjector);

        context.RegisterSourceOutput(
            lateInjectMethods.Collect(),
            GenerateValidator);
    }

    private static void GenerateValidator(
        SourceProductionContext context,
        ImmutableArray<InjectMethodInfo> methodInfos)
    {
        var source = ValidatorSourceBuilder.BuildLateInjectValidatorSource(methodInfos);

        context.AddSource(
            "LateInjectValidator.g.cs",
            SourceText.From(
                source,
                Encoding.UTF8));
    }

    private static void GenerateInjector(
        SourceProductionContext context,
        ImmutableArray<InjectMethodInfo> methodInfos)
    {
        var source = InjectorSourceBuilder.BuildLateInjectorSource(methodInfos);

        context.AddSource(
            "LateInjector.g.cs",
            SourceText.From(
                source,
                Encoding.UTF8));
    }

    private static bool NodeIsMethod(SyntaxNode node, CancellationToken _)
        => node is MethodDeclarationSyntax;

    private static InjectMethodInfo? TryGetMethodInfo(
        GeneratorAttributeSyntaxContext methodContext,
        CancellationToken _)
    {
        var method = (IMethodSymbol)methodContext.TargetSymbol;

        return method.ReturnsVoid == false
            ? null
            : InjectMethodInfo.FromMethodSymbol(method);
    }
}
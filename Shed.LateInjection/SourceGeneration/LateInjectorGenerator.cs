using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Shed.LateInjection.Attributes;

namespace Shed.LateInjection.SourceGeneration;

[Generator]
public class LateInjectorGenerator : IIncrementalGenerator
{
    private static readonly string fullyQualifiedLateInjectAttributeName
        = typeof(LateInjectAttribute).AssemblyQualifiedName;


    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var lateInjectMethods = context
            .SyntaxProvider
            .ForAttributeWithMetadataName(
                fullyQualifiedLateInjectAttributeName,
                NodeIsMethod,
                TryGetMethodInfo)
            .Where(static info => info is not null)
            .Select(static (info, _) => info!);

        context.RegisterSourceOutput(
            lateInjectMethods.Collect(),
            GenerateInjector);
    }

    private static void GenerateInjector(
        SourceProductionContext context,
        ImmutableArray<LateInjectMethodInfo> methodInfos)
    {
        var source = LateInjectorSourceBuilder.BuildLateInjectorSource(methodInfos);

        context.AddSource(
            "LateInjector.g.cs",
            SourceText.From(
                source,
                Encoding.UTF8));
    }

    private static bool NodeIsMethod(SyntaxNode node, CancellationToken _)
        => node is MethodDeclarationSyntax;

    private static LateInjectMethodInfo? TryGetMethodInfo(
        GeneratorAttributeSyntaxContext methodContext,
        CancellationToken _)
    {
        var method = (IMethodSymbol)methodContext.TargetSymbol;

        return method.ReturnsVoid == false
            ? null
            : LateInjectMethodInfo.FromMethodSymbol(method);
    }
}
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Shed.LateInjection.SourceGeneration;

internal sealed record LateInjectMethodInfo(
    string FullyQualifiedTypeName,
    string MethodName,
    ImmutableArray<string> ParameterTypes)
{
    public static LateInjectMethodInfo FromMethodSymbol(
        IMethodSymbol method)
    {
        var fullQualifiedTypeName = method
            .ContainingType
            .ToDisplayString(
                SymbolDisplayFormat.FullyQualifiedFormat);

        var parameterTypes = method
            .Parameters
            .Select(parameterSymbol =>
                parameterSymbol.Type.ToDisplayString(
                    SymbolDisplayFormat
                        .FullyQualifiedFormat))
            .ToImmutableArray();

        return new LateInjectMethodInfo(
            fullQualifiedTypeName,
            method.Name,
            parameterTypes);
    }
}
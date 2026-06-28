using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Shed.LateInjection.SourceGenerators;

internal sealed record InjectMethodInfo
{
    public string FullyQualifiedTypeName { get; }
    public string MethodName { get; }
    public ImmutableArray<string> ParameterTypes { get; }

    public InjectMethodInfo(
        string fullyQualifiedTypeName,
        string methodName,
        ImmutableArray<string> parameterTypes)
    {
        FullyQualifiedTypeName = fullyQualifiedTypeName;
        MethodName = methodName;
        ParameterTypes = parameterTypes;
    }

    public static InjectMethodInfo FromMethodSymbol(
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
                    SymbolDisplayFormat.FullyQualifiedFormat))
            .ToImmutableArray();

        return new InjectMethodInfo(
            fullQualifiedTypeName,
            method.Name,
            parameterTypes);
    }
}
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Shed.LateInjection.SourceGenerators.Models;

internal sealed record InjectMethodInfo : InjectMemberInfo
{
    public ImmutableArray<string> ParameterTypes { get; }

    public InjectMethodInfo(
        ITypeSymbol containingType,
        string name,
        ImmutableArray<string> parameterTypes)
        : base(
            containingType,
            name)
    {
        ParameterTypes = parameterTypes;
    }

    public InjectMethodInfo(IMethodSymbol method)
        : this(
            method.ContainingType,
            method.Name,
            method
                .Parameters
                .Select(parameterSymbol =>
                    parameterSymbol.Type.ToDisplayString(
                        SymbolDisplayFormat.FullyQualifiedFormat))
                .ToImmutableArray())
    { }
}
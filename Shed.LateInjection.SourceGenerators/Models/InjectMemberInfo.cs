using Microsoft.CodeAnalysis;

namespace Shed.LateInjection.SourceGenerators.Models;

internal record InjectMemberInfo
{
    public ITypeSymbol ContainingType { get; }

    public string Name { get; }

    public InjectMemberInfo(
        ITypeSymbol containingType,
        string name)
    {
        ContainingType = containingType;
        Name = name;
    }
}
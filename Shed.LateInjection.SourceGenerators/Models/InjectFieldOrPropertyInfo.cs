using Microsoft.CodeAnalysis;

namespace Shed.LateInjection.SourceGenerators.Models;

internal sealed record InjectFieldOrPropertyInfo : InjectMemberInfo
{
    public string TypeName { get; }

    public InjectFieldOrPropertyInfo(
        ITypeSymbol containingType,
        string name,
        string typeName)
        : base(
            containingType,
            name)
    {
        TypeName = typeName;
    }

    public InjectFieldOrPropertyInfo(IPropertySymbol property)
        : this(
            containingType: property.ContainingType,
            name: property.Name,
            typeName: property.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
    { }

    public InjectFieldOrPropertyInfo(IFieldSymbol field)
        : this(
            containingType: field.ContainingType,
            name: field.Name,
            typeName: field.Type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat))
    { }
}
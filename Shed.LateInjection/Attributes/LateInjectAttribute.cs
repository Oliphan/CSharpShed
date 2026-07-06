namespace Shed.LateInjection.Attributes;

[AttributeUsage(
    validOn:
        AttributeTargets.Method
        | AttributeTargets.Property
        | AttributeTargets.Field,
    Inherited = false,
    AllowMultiple = false)]
public sealed class LateInjectAttribute : Attribute
{ }
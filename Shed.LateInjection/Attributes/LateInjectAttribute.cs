namespace Shed.LateInjection.Attributes;

[AttributeUsage(
    validOn: AttributeTargets.Method,
    Inherited = false,
    AllowMultiple = false)]
public sealed class LateInjectAttribute : Attribute
{ }
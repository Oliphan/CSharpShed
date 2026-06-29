namespace Shed.LateInjection.Abstractions;

public interface ILateInjector
{
    void LateInject(object instance);
}
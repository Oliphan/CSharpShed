namespace Shed.LateInjection.Abstractions;

public interface ILateInjector
{
    void Inject(object instance);
}
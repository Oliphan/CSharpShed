namespace Shed.LateInjection.Abstractions;

public interface ILateInjectable
{
    void LateInject(IServiceProvider serviceProvider);
}

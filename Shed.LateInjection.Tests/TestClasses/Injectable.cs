using Shed.LateInjection.Attributes;

namespace Shed.LateInjection.Tests.TestClasses;

public partial class Injectable
{
    [LateInject]
    public void OnLateInject2(Service service)
    {
        Service2 = service;
    }

    public Service Service1 { get; set; } = null!;
    public Service Service2 { get; set; } = null!;
    public IServiceProvider ServiceProvider { get; set; } = null!;
    [LateInject]
    public Service Service3 { get; set; } = null!;
    [LateInject]
    private Service service4 = null!;

    [LateInject]
    public void OnLateInject1(Service service, IServiceProvider serviceProvider)
    {
        Service1 = service;
        ServiceProvider = serviceProvider;
    }

    public Service GetService4()
        => service4;
}
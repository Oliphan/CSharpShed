using Shed.LateInjection.Attributes;

namespace Shed.LateInjection.Tests.TestClasses;

public class Injectable
{
    public Service Service { get; set; } = default!;

    [LateInject]
    public void Inject(Service service)
    {
        Service = service;
    }
}
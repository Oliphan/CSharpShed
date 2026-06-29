using Shed.LateInjection.Abstractions;
using Shed.LateInjection.Attributes;

namespace Shed.LateInjection.Tests.TestClasses;

public class Injectable
{
    public Service Service { get; set; } = default!;
    public ILateInjector LateInjector { get; set; } = default!;

    [LateInject]
    public void Inject(Service service, ILateInjector lateInjector)
    {
        Service = service;
        LateInjector = lateInjector;
    }
}
using Microsoft.Extensions.DependencyInjection;
using Shed.LateInjection.Attributes;
using Shed.LateInjection.Generated;
using Shed.LateInjection.Tests.TestClasses;

namespace Shed.LateInjection.Tests;

public class LateInjectorTests
{
    [Fact]
    public void Inject_InjectsDependencies()
    {
        var services = new ServiceCollection()
            .AddSingleton<Service>()
            .BuildServiceProvider();

        var injectable = new Injectable();

        LateInjector.Inject(injectable, services);
        
        Assert.NotNull(injectable.Service);
        Assert.IsType<Service>(injectable.Service);
    }
}
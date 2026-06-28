using Microsoft.Extensions.DependencyInjection;
using Shed.LateInjection.Attributes;
using Shed.LateInjection.Generated;

namespace Shed.LateInjection.Tests;

public class InjectionGenerationTest
{
    public class Service();

    public class Injectable
    {
        private Service service = default!;

        [LateInject]
        public void Inject(Service service)
        {
            this.service = service;
        }

        public Service GetService()
            => service;
    }


    [Fact]
    public void InjectionTest()
    {
        var services = new ServiceCollection()
            .AddSingleton<Service>()
            .BuildServiceProvider();

        var injectable = new Injectable();

        LateInjector.Inject(injectable, services);
    }
}
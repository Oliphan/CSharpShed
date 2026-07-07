using Microsoft.Extensions.DependencyInjection;
using Shed.LateInjection.Tests.TestClasses;

namespace Shed.LateInjection.Tests;

public class LateInjectorTests
{
    [Fact]
    public void Inject_InjectsDependencies()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddTransient<Service>()
            .BuildServiceProvider();

        var injectable = new InjectableWrapper.Injectable();

        // Act
        services.LateInject(injectable);

        // Assert
        Assert.NotNull(injectable.Service1);
        Assert.NotNull(injectable.Service2);
        Assert.NotNull(injectable.ServiceProvider);
        Assert.NotNull(injectable.Service3);
        Assert.NotNull(injectable.GetService4());
        Assert.NotSame(injectable.Service1, injectable.Service2);
        Assert.NotSame(injectable.Service1, injectable.Service3);
        Assert.NotSame(injectable.Service1, injectable.GetService4());
        Assert.NotSame(injectable.Service2, injectable.Service3);
        Assert.NotSame(injectable.Service2, injectable.GetService4());
        Assert.NotSame(injectable.Service3, injectable.GetService4());
    }
}
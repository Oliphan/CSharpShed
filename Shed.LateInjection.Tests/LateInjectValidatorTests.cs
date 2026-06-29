using Microsoft.Extensions.DependencyInjection;
using Shed.LateInjection.Attributes;
using Shed.LateInjection.Generated;
using Shed.LateInjection.Tests.TestClasses;

namespace Shed.LateInjection.Tests;

public class LateInjectValidatorTests
{
    [Fact]
    public void Validate_WhenRequiredDependenciesAreAvailable_ReturnsWithoutThrowing()
    {
        // Arrange
        var services = new ServiceCollection()
            .AddSingleton<Service>()
            .BuildServiceProvider();

        // Act
        var exception = Record.Exception(() => LateInjectValidator.Validate(services));

        // Assert
        Assert.Null(exception);
    }

    [Fact]
    public void Validate_WhenRequiredDependenciesAreNotAvailable_ThrowsInvalidOperationException()
    {
        // Arrange
        var services = new ServiceCollection()
            .BuildServiceProvider();

        // Act and Assert
        Assert.Throws<InvalidOperationException>(() => LateInjectValidator.Validate(services));
    }
}
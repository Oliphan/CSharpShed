using Microsoft.Extensions.DependencyInjection;
using Shed.LateInjection.Abstractions;

namespace Shed.LateInjection;

public static class ServiceProviderExtensions
{
    public static void LateInject(
        this IServiceProvider serviceProvider,
        object obj)
    {
        if (obj is ILateInjectable injectable)
            injectable.LateInject(serviceProvider);
    }

    public static void LateInject(
        this IServiceProvider serviceProvider,
        ILateInjectable lateInjectable)
        => lateInjectable.LateInject(serviceProvider);
}
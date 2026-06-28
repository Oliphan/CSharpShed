using System.Collections.Immutable;
using System.Text;

namespace Shed.LateInjection.SourceGenerators;

internal sealed class InjectorSourceBuilder
{
    public static string BuildLateInjectorSource(
        ImmutableArray<InjectMethodInfo> methodInfos)
    {
        var builder = new StringBuilder();

        builder.AppendLine(
            """
            using System;
            using Microsoft.Extensions.DependencyInjection;

            namespace Shed.LateInjection.Generated;

            public static class LateInjector
            {
                public static void Inject(
                    object instance,
                    IServiceProvider services)
                {
                    switch(instance)
                    {
            """);

        foreach (var methodInfo in methodInfos)
        {
            BuildSwitchCase(builder, methodInfo);
        }

        builder.AppendLine(
            """
                        default:
                            break;
                    }
                }
            }
            """);

        return builder.ToString();
    }


    private static void BuildSwitchCase(
        StringBuilder builder,
        InjectMethodInfo methodInfo)
    {
        var args = string.Join(
                ",\n                ",
                methodInfo
                    .ParameterTypes
                    .Select(
                        paramType => $"services.GetRequiredService<{paramType}>()"));

        builder.AppendLine(
            $"""
                        case {methodInfo.FullyQualifiedTypeName} target:
                            target.{methodInfo.MethodName}(
                                {args});
                            break;
            """);
    }
}
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

            internal class LateInjector(IServiceProvider services) : ILateInjector
            {
                public void LateInject(object instance)
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
                    .Select(BuildGetServiceLine));

        builder.AppendLine(
            $"""
                        case {methodInfo.FullyQualifiedTypeName} target:
                            target.{methodInfo.MethodName}(
                                {args});
                            break;
            """);
    }

    private static string BuildGetServiceLine(string paramType)
        => paramType == "Shed.LateInjection.ILateInjector"
            ? "this"
            : $"services.GetRequiredService<{paramType}>()";
}
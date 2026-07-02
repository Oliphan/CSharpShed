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
            #pragma warning disable CS9113 // Parameter is unread
            using System;
            using Microsoft.Extensions.DependencyInjection;
            using Shed.LateInjection.Abstractions;

            namespace Shed.LateInjection.Generated;

            internal sealed class LateInjector(IServiceProvider services) : ILateInjector
            {
                public void Inject(object instance)
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
            #pragma warning restore CS9113 // Parameter is unread
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
        => paramType == "global::Shed.LateInjection.Abstractions.ILateInjector"
            ? "this"
            : $"services.GetRequiredService<{paramType}>()";
}
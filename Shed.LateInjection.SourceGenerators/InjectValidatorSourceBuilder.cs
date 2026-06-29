using System.Collections.Immutable;
using System.Text;

namespace Shed.LateInjection.SourceGenerators;

internal sealed class ValidatorSourceBuilder
{
    public static string BuildLateInjectValidatorSource(
        ImmutableArray<InjectMethodInfo> methodInfos)
    {
        var builder = new StringBuilder();

        var paramTypes = methodInfos
            .SelectMany(methodInfo => methodInfo.ParameterTypes)
            .Distinct();

        builder.AppendLine(
            """
            using System;
            using Microsoft.Extensions.DependencyInjection;

            namespace Shed.LateInjection.Generated;

            public static class LateInjectValidator
            {
                public static void Validate(
                    IServiceProvider services)
                {
                    var serviceProviderIsService = services.GetRequiredService<IServiceProviderIsService>();
            """);

        foreach (var paramType in paramTypes)
        {
            builder.Append("ThrowIfNotAvailable<");
            builder.Append(paramType);
            builder.AppendLine(">(serviceProviderIsService);");
        }

        builder.AppendLine(
            """
                }

                private static void ThrowIfNotAvailable<T>(IServiceProviderIsService serviceProviderIsService)
                {
                    if (!serviceProviderIsService.IsService(typeof(T)))
                        throw new InvalidOperationException(
                            $"{typeof(T).FullName} cannot be resolved from service provider, "
                            + "but is required by some LateInject methods. Please add a binding or remove the parameter.");
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
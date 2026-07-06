using System.Collections.Immutable;
using System.Text;
using Shed.LateInjection.SourceGenerators.Models;

namespace Shed.LateInjection.SourceGenerators;

internal sealed class ValidatorSourceBuilder
{
    public static string BuildLateInjectValidatorSource(
        ImmutableArray<InjectMemberInfo> memberInfos)
    {
        var builder = new StringBuilder();

        var allTypesUsedInInjections = memberInfos
            .SelectMany(
                static memberInfo =>
                {
                    if (memberInfo is InjectMethodInfo methodInfo)
                        return methodInfo.ParameterTypes;

                    if (memberInfo is InjectFieldOrPropertyInfo fieldOrPropertyInfo)
                        return [fieldOrPropertyInfo.TypeName];
                    
                    throw new NotImplementedException();
                })
            .Distinct();

        builder.AppendLine(
            """
            #pragma warning disable CS9113 // Parameter is unread
            using System;
            using Microsoft.Extensions.DependencyInjection;

            namespace Shed.LateInjection.Generated;

            internal static class LateInjectValidator
            {
                public static void Validate(
                    IServiceProvider services)
                {
                    var serviceProviderIsService = services.GetRequiredService<IServiceProviderIsService>();
            """);

        foreach (var paramType in allTypesUsedInInjections)
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

            #pragma warning restore CS9113 // Parameter is unread
            """);

        return builder.ToString();
    }
}
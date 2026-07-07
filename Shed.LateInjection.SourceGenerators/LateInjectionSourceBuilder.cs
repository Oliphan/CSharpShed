using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Shed.LateInjection.SourceGenerators.Models;
using Shed.LateInjection.SourceGenerators.Utility;
using Shed.ResourceManagement.Disposal;

namespace Shed.LateInjection.SourceGenerators;

internal sealed class LateInjectionSourceBuilder
{
    private const string GLOBAL_PREFIX = "global::";

    public static string BuildLateInjectionSource(
        ImmutableArray<InjectTypeInfo> typeInfos)
    {
        ScopeManagingStringBuilder builder = new(
            new StringBuilder(),
            new Indentation());

        builder.AppendLine(
            """
            using System;
            using Microsoft.Extensions.DependencyInjection;
            using Shed.LateInjection.Abstractions;

            """);

        foreach (var typeInfo in typeInfos)
        {
            BuildTypeInjection(
                builder,
                typeInfo);
        }

        return builder.ToString();
    }

    private static void BuildTypeInjection(
        ScopeManagingStringBuilder builder,
        InjectTypeInfo typeInfo)
    {
        var typeSymbol = typeInfo.TypeSymbol;
        using (GetNamespaceScope(builder, typeSymbol.ContainingNamespace))
        {
            var nestedTypes = new Stack<ITypeSymbol>();
            var currentType = typeInfo.TypeSymbol;
            while (currentType != null)
            {
                nestedTypes.Push(currentType);
                currentType = currentType.ContainingType;
            }
            
            using (GetTypeStackScope(nestedTypes, builder))
            {
                BuildLateInjectableImplementation(builder, typeInfo.MemberInfos);
                BuildLateInjectMethod(builder, typeInfo.MemberInfos);
            }
        }
    }

    private static void BuildLateInjectableImplementation(
        ScopeManagingStringBuilder builder,
        ImmutableArray<InjectMemberInfo> members)
    {
        builder.AppendIndentedLine(
            "void ILateInjectable.LateInject(IServiceProvider services)");
        using (var lateInjectIndent = builder.IndentScope())
        {
            builder.AppendIndentedLine("=> LateInject(");
            using (var paramsIndent = builder.IndentScope())
            {
                var typeNames = CollectTypeNames(members);

                var lines = typeNames.Select(
                    type => $"services.GetRequiredService<{type}>()");

                AppendIndentedWithCommaSeparators(builder, lines);
                builder.AppendLine(");");
            }
        }
    }

    private static void BuildLateInjectMethod(
        ScopeManagingStringBuilder builder,
        ImmutableArray<InjectMemberInfo> members)
    {
        builder.AppendIndentedLine("public void LateInject(");

        using (var paramsIndent = builder.IndentScope())
        {
            var typeNames = CollectTypeNames(members);

            var lines = typeNames.Select((type, index) => $"{type} p{index}");

            AppendIndentedWithCommaSeparators(builder, lines);
            builder.AppendLine(")");
        }
        
        using (var methodBodyIndent = builder.CurlyScope())
        {
            int currentParam = 0;
            
            foreach (var member in members)
            {
                if (member is InjectMethodInfo method)
                {
                    var name = method.Name;

                    builder.AppendIndentedLine($"this.{name}(");
                    using (var paramsIndent = builder.IndentScope())
                    {
                        var lines = method
                            .ParameterTypes
                            .Select(type => $"p{currentParam++}");

                        AppendIndentedWithCommaSeparators(builder, lines);

                        builder.AppendLine(");");
                    }
                }
                if (member is InjectFieldOrPropertyInfo fieldOrProperty)
                {
                    var name = fieldOrProperty.Name;

                    builder.AppendIndentedLine($"this.{name} = p{currentParam++};");
                }
            }
        }
    }

    private static List<string> CollectTypeNames(ImmutableArray<InjectMemberInfo> memberInfos)
    {
        List<string> typeNames = [];
    
        foreach (var member in memberInfos)
        {
            if (member is InjectMethodInfo method)
            {
                typeNames.AddRange(method.ParameterTypes);
            }
            if (member is InjectFieldOrPropertyInfo fieldOrProperty)
            {
                typeNames.Add(fieldOrProperty.TypeName);
            }
        }

        return typeNames;
    }

    private static void AppendIndentedWithCommaSeparators(
        ScopeManagingStringBuilder builder,
        IEnumerable<string> lines)
    {
        var lineEnumerator = lines.GetEnumerator();
        if (!lineEnumerator.MoveNext())
            return;

        var current = lineEnumerator.Current;
        while (lineEnumerator.MoveNext())
        {
            builder.AppendIndented(current);
            builder.AppendLine(",");
            current = lineEnumerator.Current;
        }
        builder.AppendIndented(current);
    }

    private static IDisposable? GetNamespaceScope(
        ScopeManagingStringBuilder builder,
        INamespaceSymbol? namespaceSymbol)
    {
        if (namespaceSymbol == null)
            return null;

        var namespaceString = namespaceSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
        namespaceString = namespaceString.StartsWith(GLOBAL_PREFIX)
            ? namespaceString.Substring(GLOBAL_PREFIX.Length)
            : namespaceString;

        if (string.IsNullOrWhiteSpace(namespaceString))
            return null;
        
        builder.AppendLine($"namespace {namespaceString}");
        
        return builder.CurlyScope();
    }

    private static IDisposable GetTypeStackScope(
        Stack<ITypeSymbol> types,
        ScopeManagingStringBuilder builder)
    {
        var currentType = types.Pop();
        var typeDeclarationKeyword = GetTypeDeclarationKeyword(currentType);
        var accessibilityModifier = currentType
            .DeclaredAccessibility
            .ToString()
            .ToLower();
        
        var lateInjectableInheritance = types.Count == 0
            ? " : ILateInjectable"
            : string.Empty;

        // Replicate the type declaration with 'partial'
        builder.AppendIndentedLine(
            $"{accessibilityModifier} partial {typeDeclarationKeyword} {currentType.Name}{lateInjectableInheritance}");
        
        var typeScope = builder.CurlyScope();
        if (types.Count == 0)
            return typeScope;

        var internalScope = GetTypeStackScope(types, builder);

        return new DisposeHandler(() =>
        {
            internalScope.Dispose();
            typeScope.Dispose();
        });
    }

    private static string GetTypeDeclarationKeyword(ITypeSymbol type)
    {
        if (type.IsRecord)
            throw new InvalidOperationException(
                "Cannot late inject records. They are immutable.");

        return type.TypeKind switch
        {
            TypeKind.Class => "class",
            TypeKind.Struct => "struct",
            TypeKind.Interface => "interface",
            _ => "class"
        };
    }
}
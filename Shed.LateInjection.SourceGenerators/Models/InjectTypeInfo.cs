using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Shed.LateInjection.SourceGenerators.Models;

internal sealed record InjectTypeInfo
{
    public ITypeSymbol TypeSymbol { get; }

    public ImmutableArray<InjectMemberInfo> MemberInfos { get; }

    public InjectTypeInfo(
        ITypeSymbol typeSymbol,
        ImmutableArray<InjectMemberInfo> memberInfos)
    {
        TypeSymbol = typeSymbol;
        MemberInfos = memberInfos;
    }
}
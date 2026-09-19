using System.Collections.Immutable;
using IdelPog.Combat.Stat.Contracts.Command;

namespace IdelPog.Combat.Stat.Contracts.Response
{
    public readonly record struct StatConfigurationResponse
    {
        public required StatConfiguration StatConfiguration { get; init; }
        public required ImmutableArray<StatBinding> StatBindings { get; init; }
    }
}
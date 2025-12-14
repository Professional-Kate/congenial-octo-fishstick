using IdelPog.Combat.Contracts.Enum;
using IdelPog.Core.Contracts;
using IdelPog.Core.Progression;

namespace IdelPog.Combat.Contracts.Response
{
    public readonly record struct ArenaCreationResponse
    {
        public required ArenaType ArenaType { get; init; }
        public required ReadOnlyLevelable ReadOnlyLevelable { get; init; }
        public required Information Information { get; init; }
    }
}
using IdelPog.Combat.Core.Contracts.Enum;
using IdelPog.Combat.Stat.Contracts.Enum;

namespace IdelPog.Combat.Core.Logging.Contracts
{
    public readonly record struct ReadOnlyStrategy
    {
        public required TargetingPreference TargetingPreference { get; init; }
        public required StatType StatType { get; init; }
        public required TargetingType TargetingType { get; init; } 
    }
}
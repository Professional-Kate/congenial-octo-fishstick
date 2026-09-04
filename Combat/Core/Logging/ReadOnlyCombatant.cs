using IdelPog.Combat.Core.Contracts.Card;
using IdelPog.Combat.Core.Contracts.Enum;

namespace IdelPog.Combat.Core.Logging
{
    public readonly record struct ReadOnlyCombatant
    {
        public required byte InstanceID { get; init; }
        public required byte CombatantID { get; init; }
        public required HealthCard HealthCard { get; init; }
        public required AgilityCard AgilityCard { get; init; }
        public required TargetingType TargetingType { get; init; }
        public required bool IsAlive { get; init; }
    }
}
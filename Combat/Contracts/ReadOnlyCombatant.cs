using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Contracts
{
    public readonly record struct ReadOnlyCombatant
    {
        public required byte InstanceID { get; init; }
        public required byte CombatantID { get; init; }
        public required StatCard StatCard { get; init; }
        public required AgilityCard AgilityCard { get; init; }
        public required TargetingType TargetingType { get; init; }
        public required bool IsAlive { get; init; }
    }
}
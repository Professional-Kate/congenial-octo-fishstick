using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Core.Event.Trigger.Contracts
{
    public readonly record struct CombatantDeathData
    {
        public required byte DeadCombatantID { get; init; }
        public required TargetingType CombatantTargetingType { get; init; }
    }
}
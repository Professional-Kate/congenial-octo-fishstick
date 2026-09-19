using IdelPog.Combat.Core.Contracts.Enum;

namespace IdelPog.Combat.Core.Event.Trigger.Contracts
{
    public readonly record struct CombatantCastCompleteData
    {
        public required TargetingType CombatantTargetingType { get; init; }
    }
}
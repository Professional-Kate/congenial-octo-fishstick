using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Core.Event.Trigger.Contracts
{
    public readonly record struct CombatantCastCompleteData
    {
        public required byte CastingCombatantID { get; init; }
        public required TargetingType CombatantTargetingType { get; init; }
    }
}
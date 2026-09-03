using IdelPog.Combat.Core.Contracts.Enum;

namespace IdelPog.Combat.Core.Event.Trigger.Contracts
{
    public readonly record struct CombatantDamagedData
    {
        public required byte InitiatingCombatantID { get; init; }
        public required byte DamagedCombatantID { get; init; }
        public required TargetingType DamagedCombatantTargetingType { get; init; }
        public required uint DamageValue { get; init; }
    }
}
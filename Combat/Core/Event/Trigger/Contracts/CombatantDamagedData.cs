using IdelPog.Combat.Combatant.Runtime.Entities;

namespace IdelPog.Combat.Core.Event.Trigger.Contracts
{
    public readonly record struct CombatantDamagedData
    {
        public required CombatantEntity InitiatingCombatant { get; init; }
        public required CombatantEntity DamagedCombatant { get; init; }
        public required uint DamageValue { get; init; }
    }
}
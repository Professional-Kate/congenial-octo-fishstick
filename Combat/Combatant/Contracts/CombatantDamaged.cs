using IdelPog.Combat.Combatant.Runtime.Entities;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Combatant.Contracts
{
    public readonly record struct CombatantDamaged : IComponent
    {
        public required CombatantEntity InitiatingCombatant { get; init; }
        public required uint DamageValue { get; init; }
    }
}
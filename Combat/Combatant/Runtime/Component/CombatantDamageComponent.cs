using IdelPog.ECS.Component;

namespace IdelPog.Combat.Combatant.Runtime.Component
{
    public readonly record struct CombatantDamageComponent : IComponent
    {
        public required byte CombatantID { get; init; }
        public required uint DamageValue { get; init; }
    }
}
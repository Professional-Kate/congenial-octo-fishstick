using IdelPog.ECS.Component;

namespace IdelPog.Combat.Combatant.Contracts
{
    public readonly record struct CombatantDamaged : IComponent
    {
        public required byte InstanceID { get; init; }
        public required uint DamageValue { get; init; }
    }
}
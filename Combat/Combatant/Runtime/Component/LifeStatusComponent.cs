using IdelPog.ECS.Component;

namespace IdelPog.Combat.Combatant.Runtime.Component
{
    public readonly record struct LifeStatusComponent : IComponent
    {
        public required bool IsAlive { get; init; }
    }
}
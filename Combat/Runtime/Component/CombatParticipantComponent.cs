using IdelPog.Combat.Runtime.Entities.Combatant;
using IdelPog.ECS.Component;

namespace IdelPog.Combat.Runtime.Component
{
    /// <summary>
    /// Should be assigned to a <see cref="CombatantEntity"/> to mark this entity is participating in combat
    /// </summary>
    public readonly record struct CombatParticipantComponent : IComponent;
}
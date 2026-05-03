using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Event.Interface;

namespace IdelPog.Combat.Event
{
    /// <summary>
    /// Handles any <see cref="ICombatEvent"/> that just does damage to an enemy
    /// </summary>
    public readonly record struct DirectDamageEvent : ICombatEvent
    {
        public EventType EventType => EventType.DIRECT_DAMAGE;
        public required AbilityType AbilityType { get; init; }
        public required double Tick { get; init; }
        public required byte AttackerID { get; init; }
    }
}
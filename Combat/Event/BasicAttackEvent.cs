using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Event.Interface;

namespace IdelPog.Combat.Event
{
    /// <summary>
    /// Handles any <see cref="ICombatEvent"/> that just does damage to an enemy
    /// </summary>
    public readonly record struct BasicAttackEvent : ICombatEvent
    {
        public EventType EventType => EventType.BASIC_ATTACK;
        public required AbilityType AbilityType { get; init; }
        public required double Tick { get; init; }
        public required byte AttackerID { get; init; }
    }
}
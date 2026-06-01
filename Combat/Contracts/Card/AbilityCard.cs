using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Event;

namespace IdelPog.Combat.Contracts.Card
{
    public readonly record struct AbilityCard
    { 
        public required AbilityType AbilityType { get; init; }
        public required EventType EventType { get; init; }
        public required byte AbilitySlots { get; init; }
        public required uint Cooldown { get; init; }
        public required uint CastTime { get; init; }
    }
}
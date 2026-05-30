using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Event;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Command
{
    public readonly record struct AbilityCreation
    {
        public required Information Information { get; init; }
        public required AbilityType AbilityType { get; init; }
        public required EventType EventType { get; init; }
        public required byte AbilitySlots { get; init; }
        public required ElementalDamageCard ElementalDamageCard { get; init; }
        public required PhysicalDamageCard PhysicalDamageCard { get; init; }
        public required uint Cooldown { get; init; }
        public required uint CastTime { get; init; }
    }
}
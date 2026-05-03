using IdelPog.Combat.Contracts.Ability;
using IdelPog.Combat.Contracts.Card;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Command
{
    public readonly record struct AbilityCreation
    {
        public required Information Information { get; init; }
        public required AbilityType AbilityType { get; init; }
        public required byte AbilitySlots { get; init; }
        public required DamageCard DamageCard { get; init; }
        public required uint Cooldown { get; init; }
    }
}
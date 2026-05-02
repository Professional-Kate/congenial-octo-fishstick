using IdelPog.Combat.Contracts.Ability;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Response
{
    public readonly record struct AbilityCreationResponse
    {
        public required Information Information { get; init; }
        public required AbilityType AbilityType { get; init; }
        public required byte AbilitySlots { get; init; }
        public required uint Cooldown { get; init; }
        public required uint Damage { get; init; }
    }
}
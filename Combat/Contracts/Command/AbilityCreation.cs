using IdelPog.Combat.Contracts.Card;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Command
{
    public readonly record struct AbilityCreation
    {
        public required Information Information { get; init; }
        public required AbilityCard AbilityCard { get; init; }
        public required ElementalDamageCard ElementalDamageCard { get; init; }
        public required PhysicalDamageCard PhysicalDamageCard { get; init; }
    }
}
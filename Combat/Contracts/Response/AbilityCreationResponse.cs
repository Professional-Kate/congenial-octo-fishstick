using IdelPog.Combat.Contracts.Card;
using IdelPog.Combat.Contracts.Enum;
using IdelPog.Combat.Event;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Response
{
    public readonly record struct AbilityCreationResponse
    {
        public required Information Information { get; init; }
        public required AbilityType AbilityType { get; init; }
        public required EventType EventType { get; init; }
        public required ElementalDamageCard ElementalDamageCard { get; init; }
    }
}
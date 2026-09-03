using System.Collections.Immutable;
using IdelPog.Combat.Core.Contracts.Card;

namespace IdelPog.Combat.Ability.Model
{
    public readonly record struct AbilityDefinition
    {
        public required AbilityCard AbilityCard { get; init; }
        public required TriggerCard TriggerCard { get; init; }
        public required ImmutableArray<AbilityStageCard> AbilityStages { get; init; }
    }
}
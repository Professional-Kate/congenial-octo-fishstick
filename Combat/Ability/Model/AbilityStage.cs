using IdelPog.Combat.Ability.Runtime.Component;
using IdelPog.Combat.Core.Contracts.Card;

namespace IdelPog.Combat.Ability.Model
{
    public readonly record struct AbilityStage
    {
        public required AbilityStageCard AbilityStageCards { get; init; }
        public required TargetingPreferenceComponent TargetingPreferenceComponent { get; init; }
    }
}
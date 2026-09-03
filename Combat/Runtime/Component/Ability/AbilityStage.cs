using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Runtime.Component.Ability
{
    public readonly record struct AbilityStage
    {
        public required AbilityStageCard AbilityStageCards { get; init; }
        public required TargetingPreferenceComponent TargetingPreferenceComponent { get; init; }
    }
}
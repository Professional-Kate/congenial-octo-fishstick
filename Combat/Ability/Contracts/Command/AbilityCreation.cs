using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Ability.Contracts.Command
{
    public readonly record struct AbilityCreation
    {
        public required AbilityCard AbilityCard { get; init; }
        public required TriggerCard TriggerCard { get; init; }
        public required AbilityStageCard[] AbilityStageCards { get; init; }
    }
}
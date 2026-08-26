using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Contracts.Response
{
    public readonly record struct AbilityCreationResponse
    {
        public required byte AbilityID { get; init; }
        public required AbilityCard AbilityCard { get; init; }
        public required TriggerCard TriggerCard { get; init; }
    }
}
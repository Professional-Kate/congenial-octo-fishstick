using IdelPog.Combat.Core.Contracts.Card;

namespace IdelPog.Combat.Core.Logging.Contracts
{
    public readonly record struct ReadOnlyAbility
    {
        public required AbilityCard AbilityCard { get; init; }
        public required byte InstanceID { get; init; }
        public required byte AbilityID { get; init; }
        public required ReadOnlyAbilityStage[] ReadOnlyAbilityStages { get; init; }
    }
}
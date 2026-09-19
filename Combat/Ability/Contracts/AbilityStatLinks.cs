namespace IdelPog.Combat.Ability.Contracts
{
    public readonly record struct AbilityStatLinks
    {
        public required byte AbilityDamageID { get; init; }
        public required byte AbilityHealingID { get; init; }
        public required byte RetaliationDamageID { get; init; }
        public required byte CastTimeID { get; init; }
        public required byte CooldownID { get; init; }
        public required byte AbilitySlotsID { get; init; }
    }
}
namespace IdelPog.Combat.Ability.Contracts.Response
{
    public readonly record struct AbilityEquipResponse
    { 
        public required byte CombatantID { get; init; }
    }
}
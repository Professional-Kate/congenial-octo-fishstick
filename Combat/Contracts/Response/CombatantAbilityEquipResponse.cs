namespace IdelPog.Combat.Contracts.Response
{
    public readonly record struct CombatantAbilityEquipResponse
    {
        public required byte CombatantID { get; init; }
        public required byte[] CombatantAbilityIDs { get; init; }
    }
}
namespace IdelPog.Combat.Combatant.Contracts.Response
{
    public readonly record struct AbilityEquipResponse
    { 
        public required byte CombatantID { get; init; }
    }
}
namespace IdelPog.Combat.Combatant.Contracts.Command
{
    public readonly record struct AbilityEquip
    {
        public required byte CombatantID { get; init; }
        public required EquippedAbility[] EquippedAbilities { get; init; }
    }
}
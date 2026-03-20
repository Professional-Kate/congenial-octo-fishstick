namespace IdelPog.Combat.Contracts
{
    public readonly record struct HighestAttackCombatant
    {
        public required byte CombatantID { get; init; }
        public required uint Attack { get; init; }
    }
}
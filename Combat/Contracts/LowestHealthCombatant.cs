namespace IdelPog.Combat.Contracts
{
    public readonly record struct LowestHealthCombatant
    {
        public required byte CombatantID { get; init; }
        public required uint Health { get; init; }
    }
}
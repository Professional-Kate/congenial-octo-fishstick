namespace IdelPog.Combat.Combatant.Contracts
{
    public readonly record struct CombatantStatLinks
    {
        public required byte BaseHealthID { get; init; }
        public required byte HealthID { get; init; }
        public required byte SpeedID { get; init; }
        public required byte InitiativeID { get; init; }
    }
}
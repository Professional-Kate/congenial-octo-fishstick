namespace IdelPog.Combat.Contracts.Command
{
    public readonly record struct BasicEncounterDeck
    {
        public required CombatantCreation[] FriendlyCombatantCards { get; init; }
        public required CombatantCreation[] EnemyCombatantCards { get; init; }
    }
}
namespace IdelPog.Combat.Core.Contracts.Command
{
    public readonly record struct BasicEncounterDeck
    {
        public required byte[] FriendlyCombatantIDs { get; init; }
        public required byte[] EnemyCombatantIDs { get; init; }
    }
}
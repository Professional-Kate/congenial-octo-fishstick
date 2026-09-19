namespace IdelPog.Combat.Core.Logging.Contracts
{
    public readonly record struct InitialEntities
    {
        public required ReadOnlyCombatant[] FriendlyCombatants { get; init; }
        public required ReadOnlyCombatant[] EnemyCombatants { get; init; }
        public required ReadOnlyAbility[] Abilities { get; init; }
    }
}
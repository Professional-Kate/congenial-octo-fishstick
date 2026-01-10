using IdelPog.Combat.Contracts.Enum;

namespace IdelPog.Combat.Contracts.Command
{
    public readonly record struct CombatSimulation
    {
        public required ArenaType ArenaType { get; init; }
        public required CombatantType[] FriendlyCombatants { get; init; }
        public required CombatantType[] EnemyCombatants { get; init; }
    }
}
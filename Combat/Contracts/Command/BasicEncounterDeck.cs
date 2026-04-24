using IdelPog.Combat.Contracts.Card.Combatant;

namespace IdelPog.Combat.Contracts.Command
{
    public readonly record struct BasicEncounterDeck
    {
        public required CombatantCard[] FriendlyCombatantCards { get; init; }
        public required CombatantCard[] EnemyCombatantCards { get; init; }
    }
}
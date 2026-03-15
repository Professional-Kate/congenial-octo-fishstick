using IdelPog.Combat.Contracts.Card;

namespace IdelPog.Combat.Contracts.Deck
{
    public readonly record struct BasicEncounterDeck
    {
        public required CombatantCard[] FriendlyCombatantCards { get; init; }
        public required CombatantCard[] EnemyCombatantCards { get; init; }
    }
}
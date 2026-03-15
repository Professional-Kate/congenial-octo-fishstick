using IdelPog.Combat.Contracts.Deck;

namespace IdelPog.Combat.Contracts.Response
{
    public readonly record struct EncounterResponse
    {
        public required BasicEncounterDeck BasicEncounterDeck { get; init; }
        public required bool FriendlyWin { get; init; }
    }
}
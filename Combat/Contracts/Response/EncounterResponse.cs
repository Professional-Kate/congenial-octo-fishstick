using IdelPog.Combat.Contracts.Deck;
using IdelPog.Combat.Service.Interface;

namespace IdelPog.Combat.Contracts.Response
{
    public readonly record struct EncounterResponse
    {
        public required ICombatLogReader CombatLogReader { get; init; }
        public required BasicEncounterDeck BasicEncounterDeck { get; init; }
        public required bool FriendlyWin { get; init; }
    }
}
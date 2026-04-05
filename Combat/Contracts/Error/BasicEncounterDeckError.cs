using IdelPog.Combat.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Contracts.Error
{
    public readonly record struct BasicEncounterDeckError
    {
        public required BasicEncounterDeck[] BasicEncounterDecks { get; init; }
        public required BaseError BaseError { get; init; }
    }
}
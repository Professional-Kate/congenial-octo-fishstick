using IdelPog.Combat.Core.Contracts.Command;
using IdelPog.Core.Contracts;

namespace IdelPog.Combat.Core.Contracts.Error
{
    public readonly record struct BasicEncounterDeckError
    {
        public required BasicEncounterDeck[] BasicEncounterDecks { get; init; }
        public required BaseError BaseError { get; init; }
    }
}
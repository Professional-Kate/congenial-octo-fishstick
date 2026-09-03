using IdelPog.Combat.Core.Contracts.Command;
using IdelPog.Combat.Core.Logging;

namespace IdelPog.Combat.Core.Contracts.Response
{
    public readonly record struct BasicEncounterDeckResponse
    {
        public required BasicEncounterDeck BasicEncounterDeck { get; init; }
        public required CombatStage[] CombatStages { get; init; }
        public required bool FriendlyVictory { get; init; }
    }
}
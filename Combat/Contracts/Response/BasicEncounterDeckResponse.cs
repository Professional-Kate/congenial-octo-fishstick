using IdelPog.Combat.Contracts.Command;

namespace IdelPog.Combat.Contracts.Response
{
    public readonly record struct BasicEncounterDeckResponse
    {
        public required BasicEncounterDeck BasicEncounterDeck { get; init; }
        public required CombatStage[] CombatStages { get; init; }
        public required bool FriendlyVictory { get; init; }
    }
}
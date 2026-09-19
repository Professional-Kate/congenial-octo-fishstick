using IdelPog.Combat.Core.Contracts.Command;
using IdelPog.Combat.Core.Logging.Contracts;

namespace IdelPog.Combat.Core.Contracts.Response
{
    public readonly record struct BasicEncounterDeckResponse
    {
        public required BasicEncounterDeck BasicEncounterDeck { get; init; }
        public required CombatArenaLog CombatArenaLog { get; init; }
    }
}
namespace IdelPog.Combat.Core.Logging.Contracts
{
    public readonly record struct CombatArenaLog
    {
        public required InitialEntities InitialEntities { get; init; }
        public required CombatStage[] CombatStages { get; init; }
        public required bool FriendlyVictory { get; init; }
    }
}
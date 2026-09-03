namespace IdelPog.Combat.Core.Event.Trigger.Contracts
{
    public readonly record struct AbilityTrigger
    {
        public required double Tick { get; init; }
        public required byte CombatantID { get; init; }
        public required byte AbilityID { get; init; }
    }
}
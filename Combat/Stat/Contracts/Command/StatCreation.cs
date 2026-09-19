namespace IdelPog.Combat.Stat.Contracts.Command
{
    public readonly record struct StatCreation
    {
        public required uint InitialValue { get; init; }
    }
}
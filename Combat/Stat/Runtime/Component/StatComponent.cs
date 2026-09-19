namespace IdelPog.Combat.Stat.Runtime.Component
{
    public readonly record struct StatComponent
    {
        public required byte StatID { get; init; }
        public required uint Value { get; init; }
    }
}
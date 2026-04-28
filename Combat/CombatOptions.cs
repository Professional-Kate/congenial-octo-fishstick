namespace IdelPog.Combat
{
    public readonly record struct CombatOptions
    {
        public required uint MaxIterations { get; init; }
    }
}
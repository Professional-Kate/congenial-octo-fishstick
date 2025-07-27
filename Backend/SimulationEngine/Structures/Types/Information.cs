namespace IdelPog.SimulationEngine.Structures.Types
{
    /// <summary>
    /// Contains two readonly strings that describes an objects readable name and readable description
    /// </summary>
    public readonly record struct Information
    {
        public required string Name { get; init; }
        public required string Description { get; init; }
    }
}
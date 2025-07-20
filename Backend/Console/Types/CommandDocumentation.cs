namespace Console.Types
{
    public readonly record struct CommandDocumentation
    {
        public required string Syntax { get; init; }
        public required string Description { get; init; }
    }
}
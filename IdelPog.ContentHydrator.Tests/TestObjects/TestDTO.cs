namespace ContentHydratorTests.TestObjects
{
    internal sealed record TestDTO
    {
        public required string TestString { get; init; } 

        public required int TestInt { get; init; }
    }
}
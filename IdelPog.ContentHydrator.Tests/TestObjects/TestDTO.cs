namespace ContentHydratorTests.TestObjects
{
    public sealed record TestDTO
    {
        public required string TestString { get; init; } 

        public required int TestInt { get; init; }
    }
}
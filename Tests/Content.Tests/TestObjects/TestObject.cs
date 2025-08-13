namespace IdelPog.Content.Tests.TestObjects
{
    public sealed record TestObject
    {
        public required string TestString { get; init; }

        public required int TestInt { get; init; }
    }
}
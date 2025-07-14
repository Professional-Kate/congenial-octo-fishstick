namespace IdelPog.Messaging.Assertions.Pipelines
{
    public interface IBufferAsserter
    {
        public void AssertCollection<T>(int expectedCount, IReadOnlyList<T> source);
    }
}
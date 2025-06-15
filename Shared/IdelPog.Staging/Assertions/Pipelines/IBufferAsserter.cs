namespace IdelPog.Staging.Assertions.Pipelines
{
    public interface IBufferAsserter
    {
        public void AssertCollection<T>(int expectedCount, ICollection<T> source);
    }
}
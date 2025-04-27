namespace IdelPog.Staging.Assertions.Pipelines
{
    public interface IBufferAsserter
    {
        public void CollectionAsserter<T>(int expectedCount, ICollection<T> source);
    }
}
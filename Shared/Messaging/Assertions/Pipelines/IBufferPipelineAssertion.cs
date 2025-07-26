namespace IdelPog.Messaging.Assertions.Pipelines
{
    public interface IBufferPipelineAssertion
    {
        public void AssertCollectionSize<T>(IReadOnlyList<T> source, int expected);
    }
}
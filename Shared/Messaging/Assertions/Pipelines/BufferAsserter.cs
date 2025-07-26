using IdelPog.Validation.Assertions;

namespace IdelPog.Messaging.Assertions.Pipelines
{
    public class BufferAsserter(IAssertNotNull assertNotNull, IAssertCollectionSize assertCollectionSize, IAssertValidCollectionSize assertValidCollectionSize)
        : IBufferAsserter
    {
        public void AssertCollection<T>(int expectedCount, IReadOnlyList<T> source)
        {
            assertNotNull.AssertObjectNotNull(source);
            assertValidCollectionSize.AssertValidSize(source.Count);
            assertCollectionSize.AssertSize(source.Count, expectedCount);
        }
    }
}
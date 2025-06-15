using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.Messaging.Assertions.Pipelines
{
    public class BufferAsserter(IAssertNotNull assertNotNull, IAssertCollectionSize assertCollectionSize, IAssertValidCollectionSize assertValidCollectionSize) : IBufferAsserter
    {
        public void AssertCollection<T>(int expectedCount, ICollection<T> source)
        {
            assertNotNull.AssertObjectNotNull(source);
            assertValidCollectionSize.AssertValidSize(source.Count);
            assertCollectionSize.AssertSize(source.Count, expectedCount);
        }
    }
}
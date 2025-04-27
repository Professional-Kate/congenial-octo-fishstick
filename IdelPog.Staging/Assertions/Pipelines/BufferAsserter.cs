using IdelPog.Validation.Assertions;

namespace IdelPog.Staging.Assertions.Pipelines
{
    public class BufferAsserter(IAssertNotNull assertNotNull, IAssertCollectionSize assertCollectionSize, IAssertValidCollectionSize assertValidCollectionSize) : IBufferAsserter
    {
        public void CollectionAsserter<T>(int expectedCount, ICollection<T> source)
        {
            assertNotNull.AssertObjectNotNull(source);
            assertValidCollectionSize.AssertValidSize(source.Count);
            assertCollectionSize.AssertSize(source.Count, expectedCount);
        }
    }
}
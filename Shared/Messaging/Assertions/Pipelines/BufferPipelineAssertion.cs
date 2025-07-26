using IdelPog.Validation.Assertions;

namespace IdelPog.Messaging.Assertions.Pipelines
{
    public class BufferPipelineAssertion : IBufferPipelineAssertion
    {
        private readonly IBufferAssertion _bufferAssertion;
        private readonly IAssertNotNull _assertNotNull;

        public BufferPipelineAssertion(IBufferAssertion bufferAssertion, IAssertNotNull assertNotNull)
        {
            _bufferAssertion = bufferAssertion;
            _assertNotNull = assertNotNull;
        }

        public void AssertCollectionSize<T>(IReadOnlyList<T> source, int expected)
        {
            _assertNotNull.AssertObjectNotNull(source);
            _bufferAssertion.AssertSizeIsValid(source.Count);
            _bufferAssertion.AssertCountEquals(source.Count, expected);
        }
    }
}
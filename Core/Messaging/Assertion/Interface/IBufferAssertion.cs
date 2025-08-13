using IdelPog.Core.Messaging.Buffer;

namespace IdelPog.Core.Messaging.Assertion.Interface
{
    public interface IBufferAssertion
    {
        public void AssertStateEquals(BufferState actual, BufferState expected);

        public void AssertSizeIsValid(int size);

        public void AssertCountEquals(int actual, int expected);
    }
}
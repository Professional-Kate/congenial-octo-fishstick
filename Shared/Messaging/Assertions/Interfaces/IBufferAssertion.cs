using IdelPog.Messaging.Buffer;

namespace IdelPog.Messaging.Assertions
{
    public interface IBufferAssertion
    {
        public void AssertStateEquals(BufferState actual, BufferState expected);

        public void AssertSizeIsValid(int size);

        public void AssertCountEquals(int actual, int expected);
    }
}
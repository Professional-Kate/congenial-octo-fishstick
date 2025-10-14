using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;

namespace IdelPog.Core.Messaging.Assertion
{
    public sealed class BufferAssertion : IBufferAssertion
    {
        public void AssertStateEquals(BufferState actual, BufferState expected)
        {
            if (actual != expected)
            {
                throw new InvalidBufferStateException(actual, expected);
            }
        }

        public void AssertSizeIsValid(int size)
        {
            if (size <= 0)
            {
                throw new BufferSizeInvalidException(size);
            }
        }

        public void AssertCountEquals(int actual, int expected)
        {
            if (actual != expected)
            {
                throw new BufferSizeMismatchException(actual, expected);
            }
        }
    }
}
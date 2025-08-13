using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Messaging.Assertion
{
    public class BufferAssertion : BaseAssertion, IBufferAssertion
    {
        public BufferAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertStateEquals(BufferState actual, BufferState expected)
        {
            Assert<InvalidBufferStateException>(() =>
            {
                if (actual != expected)
                {
                    throw new InvalidBufferStateException(actual, expected);
                }
            });
        }

        public void AssertSizeIsValid(int size)
        {
            Assert<BufferSizeInvalidException>(() =>
            {
                if (size <= 0)
                {
                    throw new BufferSizeInvalidException(size);
                }
            });
        }

        public void AssertCountEquals(int actual, int expected)
        {
            Assert<BufferSizeMismatchException>(() =>
            {
                if (actual != expected)
                {
                    throw new BufferSizeMismatchException(actual, expected);
                }
            });
        }
    }
}
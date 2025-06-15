using IdelPog.Messaging.Collection;
using IdelPog.Messaging.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Messaging.Assertions
{
    public class AssertBufferState(IHandler handler) : BaseAssertion<InvalidBufferStateException>(handler), IAssertBufferState
    {
        public void AssertState(BufferState expected, BufferState actual)
        {
            Assert(() =>
            {
                if (expected != actual)
                {
                    throw new InvalidBufferStateException(expected, actual);
                }
            });
        }
    }
}
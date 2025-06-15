using IdelPog.Staging.Collection;
using IdelPog.Staging.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.Staging.Assertions
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
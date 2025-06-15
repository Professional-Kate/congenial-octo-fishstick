using IdelPog.Messaging.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Messaging.Assertions
{
    public class AssertCollectionSize(IHandler handler) : BaseAssertion<BufferSizeMismatchException>(handler), IAssertCollectionSize
    {
        public void AssertSize(int expectedSize, int sourceSize)
        {
            Assert(() =>
            {
                if (expectedSize != sourceSize)
                {
                    throw new BufferSizeMismatchException(expectedSize, sourceSize);
                }
            });
        }
    }
}
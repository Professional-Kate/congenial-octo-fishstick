using IdelPog.Staging.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.Staging.Assertions
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
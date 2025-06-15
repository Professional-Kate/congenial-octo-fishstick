using IdelPog.Staging.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Staging.Assertions
{
    public class AssertValidCollectionSize(IHandler handler) : BaseAssertion<BufferSizeInvalidException>(handler), IAssertValidCollectionSize
    {
        public void AssertValidSize(int size)
        {
            Assert(() =>
            {
                if (size <= 0)
                {
                    throw new BufferSizeInvalidException(size);
                }
            });
        }
    }
}
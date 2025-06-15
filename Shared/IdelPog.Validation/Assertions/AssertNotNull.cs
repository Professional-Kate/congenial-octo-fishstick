using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.Validation.Assertions
{
    public class AssertNotNull(IHandler handler) : BaseAssertion<ArgumentNullException>(handler), IAssertNotNull
    {
        public void AssertObjectNotNull(object? objectToAssert)
        {
            Assert(() =>
            {
                ArgumentNullException.ThrowIfNull(objectToAssert);
            });
        }
    }
}
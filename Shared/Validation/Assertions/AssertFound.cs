using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Validation.Assertions
{
    public class AssertFound(IHandler handler) : BaseAssertion<NotFoundException>(handler), IAssertFound
    {
        public void AssertItemIsFound(object key, Func<bool> itemNotFound)
        {
            Assert(() =>
            {
                if (itemNotFound() == false)
                {
                    throw new NotFoundException(key);
                }
            });
        }
    }
}
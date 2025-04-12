using IdelPog.Engine.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Engine.Validation.Assertions.Interfaces;
using IdelPog.Engine.Validation.Exceptions;

namespace IdelPog.Engine.Validation.Assertions
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
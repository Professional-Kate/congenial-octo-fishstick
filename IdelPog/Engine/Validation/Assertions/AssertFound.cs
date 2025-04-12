using IdelPog.Engine.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Engine.Validation.Assertions.Interfaces;
using IdelPog.Engine.Validation.Exceptions;

namespace IdelPog.Engine.Validation.Assertions
{
    public class AssertFound : BaseAssertion<NotFoundException>, IAssertFound
    {
        public AssertFound(IHandler handler) : base(handler) { }
        
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
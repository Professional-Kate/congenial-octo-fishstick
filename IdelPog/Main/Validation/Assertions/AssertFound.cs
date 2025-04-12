using IdelPog.Main.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Main.Validation.Assertions.Interfaces;
using IdelPog.Main.Validation.Exceptions;

namespace IdelPog.Main.Validation.Assertions
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
using IdelPogTemp.Main.Validation.Assertions.Handlers.Interfaces;
using IdelPogTemp.Main.Validation.Assertions.Interfaces;
using IdelPogTemp.Main.Validation.Exceptions;

namespace IdelPogTemp.Main.Validation.Assertions
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
using IdelPog.Validation.Handlers.Interfaces;
using IdelPog.Validation.Interfaces;

namespace IdelPog.Validation
{
    public class AssertFound : BaseAssertion, IAssertFound
    {
        public AssertFound(IHandler handler) : base(handler) { }
        
        public void AssertItemIsFound(bool itemIsFound, object key)
        {
            Assert(() =>
            {
                if (itemIsFound == false)
                {
                    throw new NotFoundException(key, GetType());
                }
            });
        }
    }
}
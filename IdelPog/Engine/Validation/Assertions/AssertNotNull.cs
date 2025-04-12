using IdelPog.Engine.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Engine.Validation.Assertions.Interfaces;

namespace IdelPog.Engine.Validation.Assertions
{
    public class AssertNotNull(IHandler handler) : BaseAssertion<ArgumentNullException>(handler), IAssertNotNull
    {
        public void AssertObjectNotNull(object objectToAssert)
        {
            Assert(() =>
            {
                if (objectToAssert == null)
                {
                    throw new ArgumentNullException();
                }
            });
        }
    }
}
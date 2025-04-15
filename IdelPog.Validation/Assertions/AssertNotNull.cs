using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.Validation.Assertions
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
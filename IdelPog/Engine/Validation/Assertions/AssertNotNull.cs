using IdelPog.Engine.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Engine.Validation.Assertions.Interfaces;

namespace IdelPog.Engine.Validation.Assertions
{
    public class AssertNotNull : BaseAssertion<ArgumentNullException>, IAssertNotNull
    {
        public AssertNotNull(IHandler handler) : base(handler) { }

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
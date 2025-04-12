using IdelPog.Main.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Main.Validation.Assertions.Interfaces;

namespace IdelPog.Main.Validation.Assertions
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
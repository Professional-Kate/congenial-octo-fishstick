using IdelPogTemp.Main.Validation.Assertions.Handlers.Interfaces;
using IdelPogTemp.Main.Validation.Assertions.Interfaces;

namespace IdelPogTemp.Main.Validation.Assertions
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
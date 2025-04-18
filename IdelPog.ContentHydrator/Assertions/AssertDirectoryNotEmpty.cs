using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace ContentHydrator.Assertions
{
    public class AssertDirectoryNotEmpty(IHandler handler) : BaseAssertion<Exception>(handler), IAssertDirectoryNotEmpty
    {
        public void AssertNotEmpty(string[] items)
        {
            Assert(() =>
            {
                if (items.Length == 0)
                {
                    throw new Exception();
                }
            });
        }
    }
}
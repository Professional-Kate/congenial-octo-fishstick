using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Validation.Assertions
{
    public class AssertNonDuplicate(IHandler handler) : BaseAssertion<DuplicateItemException>(handler), IAssertNonDuplicate
    {
        public void AssertContains(object context, Func<bool> alreadyContains)
        {
            Assert(() =>
            {
                if (alreadyContains())
                {
                    throw new DuplicateItemException(context);
                }
            });
        }
    }
}
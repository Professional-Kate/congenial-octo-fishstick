using IdelPog.Engine.Validation.Assertions.Handlers;
using IdelPog.Engine.Validation.Exceptions;

namespace IdelPog.Engine.Validation.Assertions
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
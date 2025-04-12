using IdelPog.Main.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Main.Validation.Assertions.Interfaces;
using IdelPog.Main.Validation.Exceptions;

namespace IdelPog.Main.Validation.Assertions
{
    public class AssertNonDuplicate : BaseAssertion<DuplicateItemException>, IAssertNonDuplicate
    {
        public AssertNonDuplicate(IHandler handler) : base(handler) { }

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
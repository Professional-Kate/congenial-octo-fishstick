using IdelPogTemp.Main.Validation.Assertions.Handlers.Interfaces;
using IdelPogTemp.Main.Validation.Assertions.Interfaces;
using IdelPogTemp.Main.Validation.Exceptions;

namespace IdelPogTemp.Main.Validation.Assertions
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
using System;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Validation.Assertions
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
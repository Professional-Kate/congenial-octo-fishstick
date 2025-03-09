using System;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Exceptions;
using IdelPog.Validation.Handlers.Interfaces;

namespace IdelPog.Validation.Assertions
{
    public class AssertUniqueItem : BaseAssertion, IAssertUniqueItem
    {
        public AssertUniqueItem(IHandler handler) : base(handler) { }

        public void AssertUnique(object context, Func<bool> alreadyContains)
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
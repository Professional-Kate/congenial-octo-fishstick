using System;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.Validation.Assertions
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
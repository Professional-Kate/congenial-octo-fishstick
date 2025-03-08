using System;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Handlers.Interfaces;

namespace IdelPog.Validation.Assertions
{
    public class AssertNotNull : BaseAssertion, IAssertNotNull
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
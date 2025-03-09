using System;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.Validation.Assertions
{
    public class AssertFound : BaseAssertion, IAssertFound
    {
        public AssertFound(IHandler handler) : base(handler) { }
        
        public void AssertItemIsFound(object key, Func<bool> itemNotFound)
        {
            Assert(() =>
            {
                if (itemNotFound() == false)
                {
                    throw new NotFoundException(key);
                }
            });
        }
    }
}
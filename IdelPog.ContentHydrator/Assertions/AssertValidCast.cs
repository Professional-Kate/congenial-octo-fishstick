using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace ContentHydrator.Assertions
{
    public class AssertValidCast(IHandler handler) : BaseAssertion<InvalidCastException>(handler), IAssertValidCast
    {
        public void AssertCastable<TExpected>(object objectToAssert)
        {
            Assert(() =>
            {
                if (objectToAssert is TExpected == false)
                {
                    throw new InvalidCastException();
                }
            });
        }
    }
}
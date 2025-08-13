using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Exceptions;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Console.Assertion
{
    public class ArgumentCountAssertion : BaseAssertion, IArgumentCountAssertion
    {
        public ArgumentCountAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertCount(int actualCount, int expectedCount)
        {
            Assert<InvalidArgumentCountException>(() =>
            {
                if (actualCount != expectedCount)
                {
                    throw new InvalidArgumentCountException(actualCount, expectedCount);
                }
            });
        }
    }
}
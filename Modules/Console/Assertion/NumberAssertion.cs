using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Exceptions;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Console.Assertion
{
    public class NumberAssertion : BaseAssertion, INumberAssertion
    {
        public NumberAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertNonNegative(int number)
        {
            Assert<NegativeNumberException>(() =>
            {
                if (number < 0)
                {
                    throw new NegativeNumberException(number);
                }
            });
        }

        public void AssertAllNonNegative(int[] numbers)
        {
            Assert<NegativeNumberException>(() =>
            {
                foreach (int number in numbers)
                {
                    if (number < 0)
                    {
                        throw new NegativeNumberException(number);
                    }
                }
            });
        }
    }
}
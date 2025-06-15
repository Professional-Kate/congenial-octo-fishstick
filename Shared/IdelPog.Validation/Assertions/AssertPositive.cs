using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Assertions.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Validation.Assertions
{
    public class AssertPositive(IHandler handler) : BaseAssertion<NegativeNumberException>(handler), IAssertPositive
    {
        public void AssertNumberIsPositive(params int[] numbers)
        {
            Assert(() =>
            {
                foreach (int number in numbers)
                {
                    AssertNumber(number);
                }
            });
        }

        private static void AssertNumber(int number)
        {
            if (number < 0)
            {
                throw new NegativeNumberException(number);
            }
        }
    }
}
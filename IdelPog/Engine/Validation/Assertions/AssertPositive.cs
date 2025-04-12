using IdelPog.Engine.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Engine.Validation.Assertions.Interfaces;
using IdelPog.Engine.Validation.Exceptions;

namespace IdelPog.Engine.Validation.Assertions
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
using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Exceptions;

namespace IdelPog.Console.Assertion
{
    public sealed class NumberAssertion : INumberAssertion
    {
        public void AssertNonNegative(int number)
        {
            if (number < 0)
            {
                throw new NegativeNumberException(number);
            }
        }
    }
}
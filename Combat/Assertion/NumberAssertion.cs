using IdelPog.Combat.Assertion.Interface;
using IdelPog.Combat.Exceptions;

namespace IdelPog.Combat.Assertion
{
    public sealed class NumberAssertion : INumberAssertion
    {
        public void AssertNumberNotZero(uint number, string source)
        {
            if (number == 0)
            {
                throw new NumberZeroException(source);
            }
        }

        public void AssertNumberNotZero(double number, string source)
        {
            if (number == 0)
            {
                throw new NumberZeroException(source);
            }
        }

        public void AssertNumberNotZeroOrNegative(double number, string source)
        {
            switch (number)
            {
                case 0:
                    throw new NumberZeroException(source);
                case < 0:
                    throw new NegativeNumberException(source);
            }
        }
    }
}
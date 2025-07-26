using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.SimulationEngine.Currency.Assertions
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
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public class AssertPositive(IHandler handler) : BaseAssertion<NegativeNumberException>(handler), IAssertPositive
    {
        public void AssertNumberIsPositive<T>(params int[] numbers)
        {
            Assert(() =>
            {
                foreach (int number in numbers)
                {
                    if (number < 0)
                    {
                        throw new NegativeNumberException(typeof(T));
                    }
                }
            });
        }
    }
}
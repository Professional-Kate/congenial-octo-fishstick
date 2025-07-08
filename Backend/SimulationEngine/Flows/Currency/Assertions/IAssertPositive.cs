using IdelPog.SimulationEngine.Currency.Exceptions;

namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public interface IAssertPositive
    {
        /// <summary>
        /// Asserts that the passed int, or int array, is zero or above
        /// </summary>
        /// <param name="numbers">The number you want to assert</param>
        /// <exception cref="NegativeNumberException">WIll be thrown if the number is less than zero</exception>
        public void AssertNumberIsPositive<T>(params int[] numbers);
    }
}
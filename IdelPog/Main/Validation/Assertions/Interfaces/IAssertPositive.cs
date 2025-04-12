using IdelPogTemp.Main.Validation.Exceptions;

namespace IdelPogTemp.Main.Validation.Assertions.Interfaces
{
    /// <seealso cref="AssertNumberIsPositive"/>
    public interface IAssertPositive
    {
        /// <summary>
        /// Asserts that the passed int, or int array, is zero or above
        /// </summary>
        /// <param name="numbers">The number you want to assert</param>
        /// <exception cref="NegativeNumberException">WIll be thrown if the number is less than zero</exception>
        public void AssertNumberIsPositive(params int[] numbers);
    }
}
using IdelPog.Validation.Handlers.Interfaces;

namespace IdelPog.Validation
{
    public class AssertPositive : AssertionsHandler
    {
        public AssertPositive(IHandler handler) : base(handler) { } 
        
        /// <summary>
        /// Asserts that the passed int, or int array, is zero or above
        /// </summary>
        /// <param name="numbers">The number you want to assert</param>
        /// <exception cref="NegativeNumberException">WIll be thrown if the number is less than zero</exception>
        public void AssertNumberIsPositive(params int[] numbers)
        {
            Handle(() =>
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
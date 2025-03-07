using IdelPog.Validation.Handlers.Interfaces;
using IdelPog.Validation.Interfaces;

namespace IdelPog.Validation
{
    public class AssertPositive : BaseAssertion, IAssertPositive
    {
        public AssertPositive(IHandler handler) : base(handler) { } 
        
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
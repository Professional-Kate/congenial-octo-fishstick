using IdelPogTemp.Main.Validation.Assertions.Handlers.Interfaces;
using IdelPogTemp.Main.Validation.Assertions.Interfaces;
using IdelPogTemp.Main.Validation.Exceptions;

namespace IdelPogTemp.Main.Validation.Assertions
{
    public class AssertPositive : BaseAssertion<NegativeNumberException>, IAssertPositive
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
using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Exceptions;

namespace IdelPog.Console.Assertion
{
    public sealed class ArgumentCountAssertion : IArgumentCountAssertion
    {
        public void AssertCount(int actualCount, int expectedCount)
        {
            if (actualCount != expectedCount)
            {
                throw new InvalidArgumentCountException(actualCount, expectedCount);
            }
        }
    }
}
using Console.Commands.Resolver.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace Console.Assertions
{
    public class ArgumentCountAssertion : BaseAssertion, IArgumentCountAssertion
    {
        public ArgumentCountAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertCount(int actualCount, int expectedCount)
        {
            Assert<InvalidArgumentCountException>(() =>
            {
                if (actualCount != expectedCount)
                {
                    throw new InvalidArgumentCountException(actualCount, expectedCount);
                }
            });
        }
    }
}
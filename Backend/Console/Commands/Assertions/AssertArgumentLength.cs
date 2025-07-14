using Console.Commands.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace Console.Commands.Assertions
{
    public class AssertArgumentLength(IHandler handler) : BaseAssertion<InvalidArgumentCountException>(handler), IAssertArgumentLength
    {
        public void Handle(int actualSize, int expectedSize)
        {
            Assert(() =>
            {
                if (actualSize != expectedSize)
                {
                    throw new InvalidArgumentCountException(actualSize, expectedSize);
                }
            });
        }
    }
}
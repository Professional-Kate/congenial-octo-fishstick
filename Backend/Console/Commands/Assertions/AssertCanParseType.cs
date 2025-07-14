using Console.Commands.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace Console.Commands.Assertions
{
    public class AssertCanParseType(IHandler handler) : BaseAssertion<FailedTypeParseException>(handler), IAssertCanParseType
    {
        public void Handle(bool canParse, string argument, Type typeContext)
        {
            Assert(() =>
            {
                if (canParse == false)
                {
                    throw new FailedTypeParseException(argument, typeContext);
                }
            });
        }
    }
}
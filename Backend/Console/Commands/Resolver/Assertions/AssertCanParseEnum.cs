using Console.Commands.Resolver.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace Console.Commands.Resolver.Assertions
{
    public class AssertCanParseEnum(IHandler handler) : BaseAssertion<FailedEnumParseException>(handler), IAssertCanParseEnum
    {
        public void Handle(bool canParse, string argument, string enumName)
        {
            Assert(() =>
            {
                if (canParse == false)
                {
                    throw new FailedEnumParseException(argument, enumName);
                }
            });
        }
    }
}
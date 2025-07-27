using Console.Commands.Resolver.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace Console.Assertions
{
    public class EnumParseAssertion : BaseAssertion, IEnumParseAssertion
    {
        public EnumParseAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertCanParse(bool canParse, string argument, string enumName)
        {
            Assert<FailedEnumParseException>(() =>
            {
                if (canParse == false)
                {
                    throw new FailedEnumParseException(argument, enumName);
                }
            });
        }
    }
}
using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Exceptions;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Console.Assertion
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
using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Exceptions;

namespace IdelPog.Console.Assertion
{
    public sealed class EnumParseAssertion : IEnumParseAssertion
    {
        public void AssertCanParse(bool canParse, string argument, string enumName)
        {
            if (canParse == false)
            {
                throw new FailedEnumParseException(argument, enumName);
            }
        }
    }
}
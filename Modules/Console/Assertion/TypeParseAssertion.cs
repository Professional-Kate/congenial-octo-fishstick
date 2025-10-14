using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Exceptions;

namespace IdelPog.Console.Assertion
{
    public class TypeParseAssertion : ITypeParseAssertion
    {
        public void AssertCanParse(bool canParse, string argument, Type targetType)
        {
            if (canParse == false)
            {
                throw new FailedTypeParseException(argument, targetType);
            }
        }
    }
}
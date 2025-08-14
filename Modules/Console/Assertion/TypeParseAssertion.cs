using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Exceptions;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Console.Assertion
{
    public class TypeParseAssertion : BaseAssertion, ITypeParseAssertion
    {
        public TypeParseAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertCanParse(bool canParse, string argument, Type targetType)
        {
            Assert<FailedTypeParseException>(() =>
            {
                if (canParse == false)
                {
                    throw new FailedTypeParseException(argument, targetType);
                }
            });
        }
    }
}
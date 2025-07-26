using Console.Commands.Resolver.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace Console.Assertions
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
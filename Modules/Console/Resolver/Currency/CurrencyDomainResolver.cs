using IdelPog.Console.Argument;
using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Types;

namespace IdelPog.Console.Resolver.Currency
{
    public class CurrencyDomainResolver : ICommandDomainResolver
    {
        public Domain HandledDomain => Domain.CURRENCY;

        private readonly EnumResolver<SubDomain> _subDomainResolver;
        private readonly IArgumentCountAssertion _argumentCountAssertion;

        public CurrencyDomainResolver(IArgumentCountAssertion argumentCountAssertion, IEnumParseAssertion enumParseAssertion)
        {
            _argumentCountAssertion = argumentCountAssertion;
            _subDomainResolver = new EnumResolver<SubDomain>(enumParseAssertion);
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 3);
            
            SubDomain subDomain = _subDomainResolver.Resolve(arguments[0]);
        }
    }
}
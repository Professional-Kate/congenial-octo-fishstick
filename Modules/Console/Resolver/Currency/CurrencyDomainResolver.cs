using IdelPog.Console.Argument.Interface;
using IdelPog.Console.Assertion.Interface;

namespace IdelPog.Console.Resolver.Currency
{
    internal class CurrencyDomainResolver : IDomainResolver
    {
        private readonly ISubDomainResolver _currencyUpdateResolver;
        private readonly IArgumentResolver<SubDomain> _subDomainResolver;
        private readonly IArgumentCountAssertion _argumentCountAssertion;

        public CurrencyDomainResolver(ISubDomainResolver currencyUpdateResolver, IArgumentResolver<SubDomain> subDomainResolver, IArgumentCountAssertion argumentCountAssertion)
        {
            _currencyUpdateResolver = currencyUpdateResolver;
            _subDomainResolver = subDomainResolver;
            _argumentCountAssertion = argumentCountAssertion;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 3);
            
            SubDomain subDomain = _subDomainResolver.Resolve(arguments[0]);

            switch (subDomain)
            {
                case SubDomain.CREATE:
                    // nothing right now
                    break;
                case SubDomain.ADD:
                case SubDomain.REMOVE:
                    _currencyUpdateResolver.Resolve(arguments);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(arguments), subDomain, null);
            }
            
        }
    }
}
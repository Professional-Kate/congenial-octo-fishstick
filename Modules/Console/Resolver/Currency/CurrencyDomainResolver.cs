using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Types;

namespace IdelPog.Console.Resolver.Currency
{
    public class CurrencyDomainResolver : ICommandDomainResolver
    {
        public Domain HandledDomain => Domain.CURRENCY;

        private readonly IArgumentResolverPipeline<CurrencyUpdateArguments> _currencyUpdateResolver;
        private readonly IArgumentCountAssertion _argumentCountAssertion;

        public CurrencyDomainResolver(IArgumentResolverPipeline<CurrencyUpdateArguments> currencyUpdateResolver, IArgumentCountAssertion argumentCountAssertion)
        {
            _currencyUpdateResolver = currencyUpdateResolver;
            _argumentCountAssertion = argumentCountAssertion;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 3);
            
            _currencyUpdateResolver.Resolve(arguments);
        }
    }
}
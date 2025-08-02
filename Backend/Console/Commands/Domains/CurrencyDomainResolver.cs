using Console.Assertions;
using Console.Commands.Domains.Arguments;
using Console.Commands.Resolver.Pipelines;
using Console.Types;
using IdelPog.Common.Commands;
using IdelPog.Common.Factories;
using IdelPog.Messaging.Dispatch.Single;

namespace Console.Commands.Domains
{
    public class CurrencyDomainResolver : ICommandDomainResolver
    {
        public Domain HandledDomain => Domain.CURRENCY;
        public CommandDocumentation CommandDocumentation { get; } = new()
            { Syntax = "currency <ActionType> <int> <CurrencyType>", Description = "Add or Remove an amount from any Currency!" };

        private readonly IArgumentResolverPipeline<CurrencyUpdateArguments> _currencyUpdatePipeline;
        private readonly ICurrencyUpdateFactory _currencyUpdateFactory;
        private readonly IDispatchOne<CurrencyUpdate> _currencyUpdateDispatcher;
        private readonly IArgumentCountAssertion _argumentCountAssertion;

        public CurrencyDomainResolver(IArgumentResolverPipeline<CurrencyUpdateArguments> currencyUpdatePipeline, ICurrencyUpdateFactory currencyUpdateFactory,
            IDispatchOne<CurrencyUpdate> currencyUpdateDispatcher, IArgumentCountAssertion argumentCountAssertion)
        {
            _currencyUpdatePipeline = currencyUpdatePipeline;
            _currencyUpdateFactory = currencyUpdateFactory;
            _currencyUpdateDispatcher = currencyUpdateDispatcher;
            _argumentCountAssertion = argumentCountAssertion;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 3);

            CurrencyUpdateArguments currencyUpdateArguments = _currencyUpdatePipeline.Resolve(arguments);
            CurrencyUpdate currencyUpdate = _currencyUpdateFactory.CreateCurrencyUpdate(currencyUpdateArguments.ActionType, currencyUpdateArguments.Amount, currencyUpdateArguments.CurrencyType);

            _currencyUpdateDispatcher.Dispatch(currencyUpdate);
        }
    }
}
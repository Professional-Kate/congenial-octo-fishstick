using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Command.Domain.Argument;
using IdelPog.Console.Command.Resolver.Pipeline;
using IdelPog.Console.Types;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Messaging.Dispatcher.Single;

namespace IdelPog.Console.Command.Domain
{
    public class CurrencyDomainResolver : ICommandDomainResolver
    {
        public Types.Domain HandledDomain => Types.Domain.CURRENCY;
        public CommandDocumentation CommandDocumentation { get; } = new()
            { Syntax = "currency <ActionType> <int> <CurrencyType>", Description = "Add or Remove an amount from any Currency!" };

        private readonly IArgumentResolverPipeline<CurrencyUpdateArguments> _currencyUpdatePipeline;
        private readonly IDispatchOne<CurrencyUpdate> _currencyUpdateDispatcher;
        private readonly IArgumentCountAssertion _argumentCountAssertion;

        public CurrencyDomainResolver(IArgumentResolverPipeline<CurrencyUpdateArguments> currencyUpdatePipeline, IDispatchOne<CurrencyUpdate> currencyUpdateDispatcher, IArgumentCountAssertion argumentCountAssertion)
        {
            _currencyUpdatePipeline = currencyUpdatePipeline;
            _currencyUpdateDispatcher = currencyUpdateDispatcher;
            _argumentCountAssertion = argumentCountAssertion;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _argumentCountAssertion.AssertCount(arguments.Length, 3);

            CurrencyUpdateArguments currencyUpdateArguments = _currencyUpdatePipeline.Resolve(arguments);
            CurrencyUpdate currencyUpdate = new()
            {
                Amount = currencyUpdateArguments.Amount,
                ActionType = currencyUpdateArguments.ActionType,
                CurrencyType = currencyUpdateArguments.CurrencyType
            };

            _currencyUpdateDispatcher.Dispatch(currencyUpdate);
        }
    }
}
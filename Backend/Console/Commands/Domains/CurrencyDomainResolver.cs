using Console.Commands.Resolver.Assertions;
using Console.Commands.Resolver.Pipelines;
using Console.Types;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Messaging.Dispatch;

namespace Console.Commands.Domains
{
    public class CurrencyDomainResolver : ICommandDomainResolver
    {
        public CommandDomain HandledPermission => CommandDomain.CURRENCY;
        public CommandDocumentation CommandDocumentation { get; } = new() { Syntax = "currency <ActionType> <int> <CurrencyType>", Description = "Add or Remove an amount from any Currency!"};

        private readonly IArgumentResolverPipeline<CurrencyUpdateArguments> _currencyUpdatePipeline;
        private readonly ICurrencyUpdateFactory _currencyUpdateFactory;
        private readonly IDispatchOne<CurrencyUpdate> _currencyUpdateDispatcher;
        private readonly IAssertArgumentLength _assertArgumentLength;

        public CurrencyDomainResolver(IArgumentResolverPipeline<CurrencyUpdateArguments> currencyUpdatePipeline,  ICurrencyUpdateFactory currencyUpdateFactory,  IDispatchOne<CurrencyUpdate> currencyUpdateDispatcher, IAssertArgumentLength assertArgumentLength)
        {
            _currencyUpdatePipeline = currencyUpdatePipeline;
            _currencyUpdateFactory = currencyUpdateFactory;
            _currencyUpdateDispatcher = currencyUpdateDispatcher;
            _assertArgumentLength = assertArgumentLength;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            _assertArgumentLength.Handle(arguments.Length, 3);

            CurrencyUpdateArguments currencyUpdateArguments = _currencyUpdatePipeline.Resolve(arguments);
            CurrencyUpdate currencyUpdate = _currencyUpdateFactory.CreateCurrencyUpdate(currencyUpdateArguments.ActionType, currencyUpdateArguments.Amount, currencyUpdateArguments.CurrencyType);
            
            _currencyUpdateDispatcher.Dispatch(currencyUpdate);
        }
    }
}
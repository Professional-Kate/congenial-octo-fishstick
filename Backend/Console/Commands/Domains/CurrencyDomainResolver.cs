using Console.Commands.Assertions;
using Console.Commands.Resolver.Pipelines;
using Console.Types;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Messaging.Dispatch;

namespace Console.Commands.Domains
{
    public class CurrencyDomainResolver : ICommandDomainResolver
    {
        public CommandDomain HandledDomain => CommandDomain.CURRENCY;
        
        private readonly IArgumentResolverPipeline<CurrencyUpdateArguments> _currencyUpdatePipeline;
        private readonly IAssertArgumentLength _assertArgumentLength;
        private readonly ICurrencyUpdateFactory _currencyUpdateFactory;
        private readonly IDispatchOne<CurrencyUpdate> _currencyUpdateDispatcher;

        public CurrencyDomainResolver(IArgumentResolverPipeline<CurrencyUpdateArguments> currencyUpdatePipeline,  IAssertArgumentLength assertArgumentLength,  ICurrencyUpdateFactory currencyUpdateFactory,  IDispatchOne<CurrencyUpdate> currencyUpdateDispatcher)
        {
            _currencyUpdatePipeline = currencyUpdatePipeline;
            _assertArgumentLength = assertArgumentLength;
            _currencyUpdateFactory = currencyUpdateFactory;
            _currencyUpdateDispatcher = currencyUpdateDispatcher;
        }

        public void Resolve(string[] arguments)
        {
            // TODO: need a type to store the expected size of args, and other useful details
            _assertArgumentLength.Handle(arguments.Length, 3);

            CurrencyUpdateArguments currencyUpdateArguments = _currencyUpdatePipeline.Resolve(arguments);
            CurrencyUpdate currencyUpdate = _currencyUpdateFactory.CreateCurrencyUpdate(currencyUpdateArguments.ActionType, currencyUpdateArguments.Amount, currencyUpdateArguments.CurrencyType);
            
            _currencyUpdateDispatcher.Dispatch(currencyUpdate);
        }
    }
}
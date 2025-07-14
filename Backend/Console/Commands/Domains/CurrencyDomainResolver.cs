using Console.Commands.Assertions;
using Console.Commands.Resolver;
using Console.Commands.Resolver.Pipelines;
using Console.Types;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;

namespace Console.Commands.Domains
{
    public class CurrencyDomainResolver : ICommandDomainResolver
    {
        public CommandDomain HandledDomain => CommandDomain.CURRENCY;
        
        private readonly IArgumentResolverPipeline<CurrencyUpdateArguments> _currencyUpdatePipeline;
        private readonly IAssertArgumentLength _assertArgumentLength;
        private readonly ICurrencyUpdateFactory _currencyUpdateFactory;

        public CurrencyDomainResolver(IArgumentResolverPipeline<CurrencyUpdateArguments> currencyUpdatePipeline,  IAssertArgumentLength assertArgumentLength,  ICurrencyUpdateFactory currencyUpdateFactory)
        {
            _currencyUpdatePipeline = currencyUpdatePipeline;
            _assertArgumentLength = assertArgumentLength;
            _currencyUpdateFactory = currencyUpdateFactory;
        }

        public void Resolve(string[] args)
        {
            // TODO: need a type to store the expected size of args, and other useful details
            _assertArgumentLength.Handle(args.Length, 3);

            CurrencyUpdateArguments currencyUpdateArguments = _currencyUpdatePipeline.Resolve(args);
            CurrencyUpdate currencyUpdate = _currencyUpdateFactory.CreateCurrencyUpdate(currencyUpdateArguments.ActionType, currencyUpdateArguments.Amount, currencyUpdateArguments.CurrencyType);
            
            // TODO: dispatch CurrencyUpdate
        }
    }
}
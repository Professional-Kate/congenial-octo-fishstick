using Console.Commands.Assertions;
using Console.Commands.Resolver;
using Console.Types;
using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Structures;

namespace Console.Commands.Domains
{
    public class CurrencyDomainResolver : ICommandDomainResolver
    {
        public CommandDomain HandledDomain => CommandDomain.CURRENCY;
        
        private readonly IArgumentResolver<ActionType>  _actionTypeResolver;
        private readonly IArgumentResolver<CurrencyType> _currencyTypeResolver;
        private readonly IArgumentResolver<int> _intResolver;
        private readonly IAssertArgumentLength _AssertArgumentLength;

        public CurrencyDomainResolver(IArgumentResolver<ActionType> actionTypeResolver, IArgumentResolver<CurrencyType> currencyTypeResolver, IArgumentResolver<int> intResolver,  IAssertArgumentLength assertArgumentLength)
        {
            _actionTypeResolver = actionTypeResolver;
            _currencyTypeResolver = currencyTypeResolver;
            _intResolver = intResolver;
            _AssertArgumentLength = assertArgumentLength;
        }

        public void Resolve(string action, string[] args)
        {
            // TODO: need a type to store the expected size of args, and other useful details
            _AssertArgumentLength.Handle(args.Length, 3);
            
            ActionType actionType = _actionTypeResolver.Resolve(action);
            int amount = _intResolver.Resolve(args[0]);
            CurrencyType currencyType = _currencyTypeResolver.Resolve(args[1]);

            CurrencyUpdate currencyUpdate = new() { Action = actionType, Amount = amount, CurrencyType = currencyType };
            
            // TODO: dispatch CurrencyUpdate
        }
    }
}
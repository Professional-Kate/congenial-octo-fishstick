using Console.Commands.Resolver;
using Console.Types;
using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Structures;

namespace Console.Commands.Domains
{
    public class CurrencyDomainResolver : ICommandDomainResolver
    {
        public CommandDomain HandledDomain { get; } = CommandDomain.CURRENCY;
        
        private readonly IArgumentResolver<ActionType>  _actionTypeResolver;
        private readonly IArgumentResolver<CurrencyType> _currencyTypeResolver;
        private readonly IArgumentResolver<int> _intResolver;

        public CurrencyDomainResolver(IArgumentResolver<ActionType> actionTypeResolver, IArgumentResolver<CurrencyType> currencyTypeResolver,  IArgumentResolver<int> intResolver)
        {
            _actionTypeResolver = actionTypeResolver;
            _currencyTypeResolver = currencyTypeResolver;
            _intResolver = intResolver;
        }

        public void Resolve(string action, string[] args)
        {
            // TODO: verify args are length two <CurrencyType> <int>
            
            ActionType actionType = _actionTypeResolver.Resolve(action);
            CurrencyType currencyType = _currencyTypeResolver.Resolve(args[0]);
            int amount = _intResolver.Resolve(args[1]);
            
            switch (actionType)
            {
                case ActionType.ADD: 
                    // TODO: verify args have add <int>
                    // TODO: assign CurrencyUpdate
                    break;
                case ActionType.REMOVE:
                    // TODO: verify args have remove <int>
                    // TODO: assign CurrencyUpdate
                    break;
                default: 
                    throw new ArgumentOutOfRangeException($"{action} is not a valid action for domain: {HandledDomain}");
            }
            
            // TODO: dispatch CurrencyUpdate
        }
    }
}
using IdelPog.Console.Argument.Interface;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Single;

namespace IdelPog.Console.Resolver.Currency
{
    public class CurrencyUpdateResolver : ISubDomainResolver
    {
        private readonly IArgumentResolver<ActionType> _actionTypeResolver;
        private readonly IArgumentResolver<uint> _intResolver;
        private readonly IArgumentResolver<CurrencyType> _currencyTypeResolver;
        private readonly IDispatchOne<CurrencyUpdate> _currencyUpdateDispatcher;

        public CurrencyUpdateResolver(IArgumentResolver<ActionType> actionTypeResolver, IArgumentResolver<uint> intResolver, IArgumentResolver<CurrencyType> currencyTypeResolver, IDispatchOne<CurrencyUpdate> currencyUpdateDispatcher)
        {
            _actionTypeResolver = actionTypeResolver;
            _intResolver = intResolver;
            _currencyTypeResolver = currencyTypeResolver;
            _currencyUpdateDispatcher = currencyUpdateDispatcher;
        }

        public void Resolve(ReadOnlySpan<string> arguments)
        {
            ActionType actionType = _actionTypeResolver.Resolve(arguments[0]);
            uint amount = _intResolver.Resolve(arguments[1]);
            CurrencyType currencyType = _currencyTypeResolver.Resolve(arguments[2]);
            
            _currencyUpdateDispatcher.Dispatch(new CurrencyUpdate { ActionType = actionType, Amount = amount,  CurrencyType = currencyType });
        }

        public string GetHelp()
        {
            throw new NotImplementedException();
        }
    }
}
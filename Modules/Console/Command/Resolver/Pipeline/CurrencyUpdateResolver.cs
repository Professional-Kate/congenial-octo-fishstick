using IdelPog.Console.Command.Domain.Argument;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Console.Command.Resolver.Pipeline
{
    public class CurrencyUpdateResolver : IArgumentResolverPipeline<CurrencyUpdateArguments>
    {
        private readonly IArgumentResolver<ActionType> _actionTypeResolver;
        private readonly IArgumentResolver<uint> _intResolver;
        private readonly IArgumentResolver<CurrencyType> _currencyTypeResolver;

        public CurrencyUpdateResolver(IArgumentResolver<ActionType> actionTypeResolver, IArgumentResolver<uint> intResolver,
            IArgumentResolver<CurrencyType> currencyTypeResolver)
        {
            _actionTypeResolver = actionTypeResolver;
            _intResolver = intResolver;
            _currencyTypeResolver = currencyTypeResolver;
        }

        public CurrencyUpdateArguments Resolve(ReadOnlySpan<string> arguments)
        {
            ActionType actionType = _actionTypeResolver.Resolve(arguments[0]);
            uint amount = _intResolver.Resolve(arguments[1]);
            CurrencyType currencyType = _currencyTypeResolver.Resolve(arguments[2]);

            return new CurrencyUpdateArguments
            {
                ActionType = actionType,
                Amount = amount,
                CurrencyType = currencyType
            };
        }
    }
}
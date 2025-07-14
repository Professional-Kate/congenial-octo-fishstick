using Console.Commands.Resolver;
using Console.Types;
using IdelPog.Common.Enums;

namespace Console.Commands.Domains
{
    public class CurrencyDomainResolver : ICommandDomainResolver
    {
        public CommandDomain HandledDomain { get; } = CommandDomain.CURRENCY;
        
        private readonly IArgumentResolver<CurrencyType> _currencyTypeResolver;

        public CurrencyDomainResolver(IArgumentResolver<CurrencyType> currencyTypeResolver)
        {
            _currencyTypeResolver = currencyTypeResolver;
        }

        public void Resolve(string action, string[] args)
        {
            // TODO: verify args are length two <CurrencyType> <int>
            
            string normalizedAction = action.ToLowerInvariant();
            CurrencyType currencyType = _currencyTypeResolver.Resolve(args[0]);
            
            switch (normalizedAction)
            {
                case "add": 
                    // TODO: verify args have add <int>
                    // TODO: assign CurrencyUpdate
                    break;
                case "remove":
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
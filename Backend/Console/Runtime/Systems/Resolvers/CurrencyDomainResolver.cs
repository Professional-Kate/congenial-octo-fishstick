namespace Console.Runtime.Systems.Resolvers
{
    public class CurrencyDomainResolver : ICommandDomainResolver
    {
        public string HandledDomainName { get; } = "currency";
        
        public void Resolve(string action, string[] args)
        {
            // TODO: verify args are length two <Action> <int>
            string normalizedAction = action.ToLowerInvariant();
            // TODO: initialize CurrencyUpdate
            
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
                    throw new ArgumentOutOfRangeException($"{action} is not a valid action for domain: {HandledDomainName}");
            }
            
            // TODO: dispatch CurrencyUpdate
        }
    }
}
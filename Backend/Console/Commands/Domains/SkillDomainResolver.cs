namespace Console.Commands.Domains
{
    public class SkillDomainResolver : ICommandDomainResolver
    {
        public string HandledDomainName { get; } = "skill";
        
        public void Resolve(string action, string[] args)
        {
            // TODO: verify args are length one <SkillID>
            // TODO: verify args have SkillID
            string normalizedAction = action.ToLowerInvariant();
            // TODO: initialize SkillChange
            
            switch (normalizedAction)
            {
                case "change": 
                    // TODO: assign SkillChange
                    break;
                default: 
                    throw new ArgumentOutOfRangeException($"{action} is not a valid action for domain: {HandledDomainName}");
            }
            
            // TODO: dispatch SkillChange
        }
    }
}
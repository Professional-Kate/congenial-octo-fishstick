using Console.Types;

namespace Console.Commands.Domains
{
    public class SkillDomainResolver : ICommandDomainResolver
    {
        public CommandDomain HandledDomain { get; } = CommandDomain.SKILL;
        
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
                    throw new ArgumentOutOfRangeException($"{action} is not a valid action for domain: {HandledDomain}");
            }
            
            // TODO: dispatch SkillChange
        }
    }
}
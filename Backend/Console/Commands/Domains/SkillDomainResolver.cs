using Console.Types;

namespace Console.Commands.Domains
{
    public class SkillDomainResolver : ICommandDomainResolver
    {
        public CommandDomain HandledDomain { get; } = CommandDomain.SKILL;
        
        public void Resolve(string[] arguments)
        {
            // TODO: verify args are length one <SkillID>
            // TODO: verify args have SkillID
            string normalizedAction = arguments[1].ToLowerInvariant();
            // TODO: initialize SkillChange
            
            switch (normalizedAction)
            {
                case "change": 
                    // TODO: assign SkillChange
                    break;
                default: 
                    throw new ArgumentOutOfRangeException($"{normalizedAction} is not a valid action for domain: {HandledDomain}");
            }
            
            // TODO: dispatch SkillChange
        }
    }
}
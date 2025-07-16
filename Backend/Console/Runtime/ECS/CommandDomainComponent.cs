using Console.Types;
using IdelPog.ECS.Component;

namespace Console.Runtime.ECS
{
    public readonly record struct CommandDomainComponent : IComponent<CommandDomainComponent>
    {
        public required CommandDomain AllowedCommandDomain { get; init; }
        
        public CommandDomainComponent DeepClone()
        {
            return new CommandDomainComponent { AllowedCommandDomain = AllowedCommandDomain };
        }
    }
}
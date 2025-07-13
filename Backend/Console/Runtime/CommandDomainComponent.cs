using Console.Types;
using IdelPog.ECS.Component;

namespace Console.Runtime
{
    public readonly record struct CommandDomainComponent : IComponent<CommandDomainComponent>
    {
        public required CommandDomain CommandDomain { get; init; }
        
        public CommandDomainComponent DeepClone()
        {
            return new CommandDomainComponent { CommandDomain = CommandDomain };
        }
    }
}
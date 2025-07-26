using Console.Types;
using IdelPog.ECS.Component;

namespace Console.Runtime.ECS
{
    public readonly record struct DomainComponent : IComponent<DomainComponent>
    {
        public required Domain AllowedDomain { get; init; }

        public DomainComponent DeepClone()
        {
            return new DomainComponent { AllowedDomain = AllowedDomain };
        }
    }
}
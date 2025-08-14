using IdelPog.Console.Types;
using IdelPog.ECS.Component;

namespace IdelPog.Console.Runtime.ECS
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
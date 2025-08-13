using IdelPog.Core.Contracts.Enum;
using IdelPog.ECS.Component;

namespace IdelPog.HarvestNode.Runtime.ECS
{
    public readonly record struct ResourceComponent : IComponent<ResourceComponent>
    {
        public required ResourceID ResourceID { get; init; }
        
        public ResourceComponent DeepClone()
        {
            return new ResourceComponent { ResourceID = ResourceID };
        }
    }
}
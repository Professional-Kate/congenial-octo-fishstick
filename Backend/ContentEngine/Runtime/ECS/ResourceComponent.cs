using IdelPog.Common.Enums;
using IdelPog.ECS.Component;

namespace ContentEngine.Runtime.ECS
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
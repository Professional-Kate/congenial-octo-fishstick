using IdelPog.Core.Contracts.Enum;
using IdelPog.ECS.Component;

namespace IdelPog.HarvestNode.Runtime.ECS
{
    public readonly record struct HarvestTargetComponent : IComponent<HarvestTargetComponent>
    {
        public required ItemID HarvestTarget { get; init; }
        
        public HarvestTargetComponent DeepClone()
        {
            return new HarvestTargetComponent { HarvestTarget = HarvestTarget };
        }
    }
}
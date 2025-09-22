using IdelPog.Core.Contracts.Enum;
using IdelPog.ECS.Component;
using IdelPog.Progression.Contracts;

namespace IdelPog.Progression.Runtime.ECS.Component
{
    public readonly record struct NodeUnlockRequirement : IComponent<NodeUnlockRequirement>
    {
        public required ItemID ItemID { get; init; }
        public required byte Level { get; init; }
        public required HarvestNodeUnlockResponse HarvestNodeUnlockResponse { get; init; }

        public NodeUnlockRequirement DeepClone()
        {
            return this;
        }
    }
}
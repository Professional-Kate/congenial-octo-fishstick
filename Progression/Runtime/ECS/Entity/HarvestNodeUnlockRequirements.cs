using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Handler;
using IdelPog.Progression.Runtime.ECS.Component;

namespace IdelPog.Progression.Runtime.ECS.Entity
{
    public sealed record HarvestNodeUnlockRequirements : IdelPog.ECS.Entity.Entity
    {
        public readonly ItemID ItemID;
        private QueueComponentStore<NodeUnlockRequirement> _nodeUnlockStore;

        public HarvestNodeUnlockRequirements(NodeUnlockRequirement[] unlockComponents, ItemID itemID)
            : base(new QueueComponentStore<NodeUnlockRequirement>(unlockComponents, new ThrowHandler()))
        {
            ItemID = itemID;
            _nodeUnlockStore = GetComponent<QueueComponentStore<NodeUnlockRequirement>>();
        }

        public ReadOnlySpan<NodeUnlockRequirement> GetRequirements()
        {
            return _nodeUnlockStore.ToArray();
        }

        public void RebuildStore(QueueComponentStore<NodeUnlockRequirement> store)
        {
            RemoveComponent<QueueComponentStore<NodeUnlockRequirement>>();
            AddComponent(store);
            _nodeUnlockStore = store;
        }
    }
}
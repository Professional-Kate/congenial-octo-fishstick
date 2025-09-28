using IdelPog.Core.Validation.Handler;
using IdelPog.ECS.Entity;
using IdelPog.Progression.Runtime.ECS.Component;

namespace IdelPog.Progression.Runtime.ECS
{
    public sealed record UnlockRequirementsEntity<TID, TCommand> : Entity where TCommand : struct
    {
        private readonly QueueComponentStore<NodeLevelRequirement<TID, TCommand>> _levelRequirementStore;

        public UnlockRequirementsEntity(NodeLevelRequirement<TID, TCommand>[] unlockComponents)
            : base(new QueueComponentStore<NodeLevelRequirement<TID, TCommand>>(unlockComponents, new ThrowHandler()))
        {
            _levelRequirementStore = GetComponent<QueueComponentStore<NodeLevelRequirement<TID, TCommand>>>();
        }

        public NodeLevelRequirement<TID, TCommand> Peek()
        {
            return _levelRequirementStore.Peek();
        }

        public bool TryDequeue(out NodeLevelRequirement<TID, TCommand> levelRequirement)
        {
            return _levelRequirementStore.TryDequeue(out levelRequirement);
        }

        public NodeLevelRequirement<TID, TCommand>[] ToArray()
        {
            return _levelRequirementStore.ToArray();
        }
    }
}
using IdelPog.Core.Validation.Handler;
using IdelPog.ECS.Entity;
using IdelPog.Progression.Runtime.ECS.Component;

namespace IdelPog.Progression.Runtime.ECS
{
    public sealed record UnlockRequirementsEntity<TCommand> : Entity where TCommand : struct
    {
        private readonly QueueComponentStore<NodeLevelRequirement<TCommand>> _levelRequirementStore;

        public UnlockRequirementsEntity(NodeLevelRequirement<TCommand>[] unlockComponents)
            : base(new QueueComponentStore<NodeLevelRequirement<TCommand>>(unlockComponents, new ThrowHandler()))
        {
            _levelRequirementStore = GetComponent<QueueComponentStore<NodeLevelRequirement<TCommand>>>();
        }

        public NodeLevelRequirement<TCommand> Peek()
        {
            return _levelRequirementStore.Peek();
        }

        public bool TryDequeue(out NodeLevelRequirement<TCommand> levelRequirement)
        {
            return _levelRequirementStore.TryDequeue(out levelRequirement);
        }

        public NodeLevelRequirement<TCommand>[] ToArray()
        {
            return _levelRequirementStore.ToArray();
        }
    }
}
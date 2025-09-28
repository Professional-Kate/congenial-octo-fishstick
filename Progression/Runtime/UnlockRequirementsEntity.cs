using IdelPog.Core.Validation.Handler;
using IdelPog.ECS.Entity;
using IdelPog.Progression.Runtime.Component;

namespace IdelPog.Progression.Runtime
{
    public sealed record UnlockRequirementsEntity<TID, TCommand> : Entity where TCommand : struct
    {
        private readonly QueueComponentStore<LevelRequirementComponent<TID, TCommand>> _levelRequirementStore;

        public UnlockRequirementsEntity(LevelRequirementComponent<TID, TCommand>[] unlockComponents)
            : base(new QueueComponentStore<LevelRequirementComponent<TID, TCommand>>(unlockComponents, new ThrowHandler()))
        {
            _levelRequirementStore = GetComponent<QueueComponentStore<LevelRequirementComponent<TID, TCommand>>>();
        }

        public LevelRequirementComponent<TID, TCommand> Peek()
        {
            return _levelRequirementStore.Peek();
        }

        public bool TryDequeue(out LevelRequirementComponent<TID, TCommand> levelRequirementComponent)
        {
            return _levelRequirementStore.TryDequeue(out levelRequirementComponent);
        }

        public LevelRequirementComponent<TID, TCommand>[] ToArray()
        {
            return _levelRequirementStore.ToArray();
        }
    }
}
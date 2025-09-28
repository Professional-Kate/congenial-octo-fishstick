using IdelPog.ECS.Component;

namespace IdelPog.Progression.Runtime.ECS.Component
{
    public readonly record struct LevelRequirementComponent<TID, TCommand> : IComponent<LevelRequirementComponent<TID, TCommand>> where TCommand : struct
    {
        public required TID ID { get; init; }
        public required byte Level { get; init; }
        public required TCommand OnUnlockCommand { get; init; }

        public LevelRequirementComponent<TID, TCommand> DeepClone()
        {
            return this;
        }
    }
}
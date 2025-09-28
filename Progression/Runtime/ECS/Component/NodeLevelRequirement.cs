using IdelPog.ECS.Component;

namespace IdelPog.Progression.Runtime.ECS.Component
{
    public readonly record struct NodeLevelRequirement<TID, TCommand> : IComponent<NodeLevelRequirement<TID, TCommand>> where TCommand : struct
    {
        public required TID ID { get; init; }
        public required byte Level { get; init; }
        public required TCommand OnUnlockCommand { get; init; }

        public NodeLevelRequirement<TID, TCommand> DeepClone()
        {
            return this;
        }
    }
}
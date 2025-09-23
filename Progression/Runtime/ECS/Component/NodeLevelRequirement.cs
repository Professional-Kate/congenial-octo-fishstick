using IdelPog.Core.Contracts.Enum;
using IdelPog.ECS.Component;

namespace IdelPog.Progression.Runtime.ECS.Component
{
    public readonly record struct NodeLevelRequirement<TCommand> : IComponent<NodeLevelRequirement<TCommand>> where TCommand : struct
    {
        public required SkillID SkillID { get; init; }
        public required byte Level { get; init; }
        public required TCommand OnUnlockCommand { get; init; }

        public TCommand Unlock()
        {
            return OnUnlockCommand;
        }
        
        public NodeLevelRequirement<TCommand> DeepClone()
        {
            return this;
        }
    }
}
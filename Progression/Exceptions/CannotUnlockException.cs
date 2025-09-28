using IdelPog.Progression.Runtime.ECS.Component;

namespace IdelPog.Progression.Exceptions
{
    public sealed class CannotUnlockException<TCommand> : Exception where TCommand : struct
    {
        private const string MESSAGE = "Cannot unlock {0}! Required level {1} passed level {2}";

        public readonly byte PassedLevel;
        public readonly byte RequiredLevel;
        public readonly NodeLevelRequirement<TCommand> NodeLevelRequirement;

        public CannotUnlockException(byte passedLevel, byte requiredLevel, NodeLevelRequirement<TCommand> nodeLevelRequirement) : base(string.Format(MESSAGE, nodeLevelRequirement.SkillID, requiredLevel, passedLevel))
        {
            NodeLevelRequirement = nodeLevelRequirement;
            RequiredLevel = requiredLevel;
            PassedLevel = passedLevel;
        }
    }
}
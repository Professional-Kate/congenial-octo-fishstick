using IdelPog.Progression.Runtime.ECS.Component;

namespace IdelPog.Progression.Exceptions
{
    public sealed class CannotUnlockException<TID, TCommand> : Exception where TCommand : struct
    {
        private const string MESSAGE = "Cannot unlock {0}! Required level {1} passed level {2}";

        public readonly byte PassedLevel;
        public readonly byte RequiredLevel;
        public readonly NodeLevelRequirement<TID, TCommand> NodeLevelRequirement;

        public CannotUnlockException(byte passedLevel, byte requiredLevel, NodeLevelRequirement<TID, TCommand> nodeLevelRequirement) : base(string.Format(MESSAGE, nodeLevelRequirement.ID, requiredLevel, passedLevel))
        {
            NodeLevelRequirement = nodeLevelRequirement;
            RequiredLevel = requiredLevel;
            PassedLevel = passedLevel;
        }
    }
}
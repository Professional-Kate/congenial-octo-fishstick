using IdelPog.Progression.Runtime.Component;

namespace IdelPog.Progression.Exceptions
{
    public sealed class CannotUnlockException<TID, TCommand> : Exception where TCommand : struct
    {
        private const string MESSAGE = "Cannot unlock {0}! Required level {1} passed level {2}";

        public readonly byte PassedLevel;
        public readonly byte RequiredLevel;
        public readonly LevelRequirementComponent<TID, TCommand> LevelRequirementComponent;

        public CannotUnlockException(byte passedLevel, byte requiredLevel, LevelRequirementComponent<TID, TCommand> levelRequirementComponent) : base(string.Format(MESSAGE, levelRequirementComponent.ID, requiredLevel, passedLevel))
        {
            LevelRequirementComponent = levelRequirementComponent;
            RequiredLevel = requiredLevel;
            PassedLevel = passedLevel;
        }
    }
}
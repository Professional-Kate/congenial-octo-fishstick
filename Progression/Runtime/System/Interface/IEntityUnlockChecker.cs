using IdelPog.Progression.Runtime.Component;

namespace IdelPog.Progression.Runtime.System.Interface
{
    public interface IEntityUnlockChecker<TID, TCommand> where TCommand : struct
    {
        public bool IsUnlocked(TID id, Predicate<LevelRequirementComponent<TID, TCommand>> predicate);
    }
}
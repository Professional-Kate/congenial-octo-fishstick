using IdelPog.Progression.Runtime.Component;

namespace IdelPog.Progression.Exceptions
{
    public sealed class UnsuccessfulDequeueException<TID, TCommand> : Exception where TCommand : struct
    {
        private const string MESSAGE = "Could not dequeue component!";
        
        public readonly LevelRequirementComponent<TID, TCommand> LevelRequirementComponent;

        public UnsuccessfulDequeueException(LevelRequirementComponent<TID, TCommand> levelRequirementComponent) : base(string.Format(MESSAGE))
        { 
            LevelRequirementComponent = levelRequirementComponent;
        }
    }
}
using IdelPog.Progression.Runtime.ECS.Component;

namespace IdelPog.Progression.Exceptions
{
    public sealed class UnsuccessfulDequeueException<TID, TCommand> : Exception where TCommand : struct
    {
        private const string MESSAGE = "Could not dequeue component!";
        
        public readonly NodeLevelRequirement<TID, TCommand> NodeLevelRequirement;

        public UnsuccessfulDequeueException(NodeLevelRequirement<TID, TCommand> nodeLevelRequirement) : base(string.Format(MESSAGE))
        { 
            NodeLevelRequirement = nodeLevelRequirement;
        }
    }
}
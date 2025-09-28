using IdelPog.Progression.Runtime.ECS.Component;

namespace IdelPog.Progression.Exceptions
{
    public sealed class UnsuccessfulDequeueException<TCommand> : Exception where TCommand : struct
    {
        private const string MESSAGE = "Could not dequeue component!";
        
        public readonly NodeLevelRequirement<TCommand> NodeLevelRequirement;

        public UnsuccessfulDequeueException(NodeLevelRequirement<TCommand> nodeLevelRequirement) : base(string.Format(MESSAGE))
        { 
            NodeLevelRequirement = nodeLevelRequirement;
        }
    }
}
using IdelPog.Flows.Types;
using IdelPog.Messaging.Listeners;

namespace IdelPog.Flows
{
    public interface IFlowBuilderService
    {
        public IListener ConstructFlow<TCommand, TError>(FlowDescriptor flowDescriptor);
    }
}
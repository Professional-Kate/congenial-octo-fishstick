using IdelPog.Flows.Types;
using IdelPog.Messaging.Listeners.Buffer;

namespace IdelPog.Flows.Registry
{
    public class FlowRegistryController : IBatchController<FlowDescriptor>
    {
        private readonly IBatchMediator<FlowDescriptor> _registryFlowMediator;

        public FlowRegistryController(IBatchMediator<FlowDescriptor> registryFlowMediator)
        {
            _registryFlowMediator = registryFlowMediator;
        }

        public void HandleMessages(IReadOnlyList<FlowDescriptor> messages)
        {
            _registryFlowMediator.HandleMessages(messages);
        }
    }
}
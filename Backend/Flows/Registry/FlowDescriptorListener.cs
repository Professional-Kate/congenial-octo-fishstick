using IdelPog.Flows.Types;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Listeners.Buffer;

namespace IdelPog.Flows.Registry
{
    public class FlowDescriptorListener : IBufferListener<FlowDescriptor>
    {
        private readonly IBatchController<FlowDescriptor> _registryFlowController;

        public FlowDescriptorListener(IBatchController<FlowDescriptor> registryFlowController)
        {
            _registryFlowController = registryFlowController;
        }

        public Type ListenerType => typeof(FlowDescriptor);
        
        public void Handle(IReadOnlyList<FlowDescriptor> buffer)
        {
            _registryFlowController.HandleMessages(buffer);
        }

    }
}
using IdelPog.Flows.Builder;
using IdelPog.Flows.Register;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Messenger;

namespace IdelPog.Flows.Service
{
    public class FlowConstructionService : IFlowConstructionService
    {
        private readonly IFlowRegister  _flowRegister;
        private readonly IBufferMessenger  _bufferMessenger;

        public FlowConstructionService(IFlowRegister flowRegister, IBufferMessenger bufferMessenger)
        {
            _flowRegister = flowRegister;
            _bufferMessenger = bufferMessenger;
        }

        public void ConstructAndSubscribe<TCommand, TError>(FlowDescriptor flowDescriptor)
        {
            IListener listener = _flowRegister.ConstructFlow<TCommand, TError>(flowDescriptor.DispatchMode);
            
            _bufferMessenger.Subscribe(listener);
        }
    }
}
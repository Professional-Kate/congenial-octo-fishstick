using IdelPog.Common.Repository;
using IdelPog.Flows.Types;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Messenger;
using IdelPog.Validation.Assertions;

namespace IdelPog.Flows
{
    public class FlowSubscriptionMediator : IFlowSubscriptionMediator
    {
        private readonly IAssetRepository<Type, FlowDescriptor> _flowRepository;
        private readonly IFlowBuilderService  _flowBuilderService;
        private readonly IBufferMessenger  _bufferMessenger;
        private readonly IFoundAssertion _foundAssertion;

        public FlowSubscriptionMediator(IAssetRepository<Type, FlowDescriptor> flowRepository, IFlowBuilderService flowBuilderService, IBufferMessenger bufferMessenger, IFoundAssertion foundAssertion)
        {
            _flowRepository = flowRepository;
            _flowBuilderService = flowBuilderService;
            _bufferMessenger = bufferMessenger;
            _foundAssertion = foundAssertion;
        }

        public void ConstructAndSubscribe<TCommand, TError>()
        {
            Type commandType = typeof(TCommand);
            _foundAssertion.AssertFound(commandType, _flowRepository.Contains(commandType));
            IListener listener = _flowBuilderService.ConstructFlow<TCommand, TError>(_flowRepository.Get(commandType));
            
            _bufferMessenger.Subscribe(listener);
        }
    }
}
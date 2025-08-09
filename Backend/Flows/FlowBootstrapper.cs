using System.Diagnostics.CodeAnalysis;
using IdelPog.Common.Commands;
using IdelPog.Common.Errors;
using IdelPog.Common.Repository;
using IdelPog.Common.Responses;
using IdelPog.Flows.Registry;
using IdelPog.Flows.Types;
using IdelPog.Messaging.Controller;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Messaging.Messenger;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.Flows
{
    public static class FlowBootstrapper
    {
        private static IAssetRepository<Type, FlowDescriptor>? _flowRepository { get; set; }

        public static void Initialize(IBufferMessenger bufferMessenger)
        {
            _flowRepository = new AssetRepository<Type, FlowDescriptor>();
            
            InitializeListener(bufferMessenger, _flowRepository);
        }

        private static void InitializeListener(IBufferMessenger bufferMessenger, IAssetRepository<Type, FlowDescriptor> flowRepository)
        {
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(new ThrowHandler());
            
            IBatchMediator<FlowDescriptor> registryMediator = new FlowRegistryMediator(flowRepository, uniqueAssertion);
            IBatchController<FlowDescriptor> registryController = new ManagedBatchController<FlowDescriptor>(registryMediator);
            
            FlowDescriptorListener flowDescriptorListener = new(registryController);
            bufferMessenger.Subscribe(flowDescriptorListener);
        }

        public static void InitializeFlows(IBufferMessenger bufferMessenger)
        {
            ValidateRepositoryNotNull();
            IFoundAssertion foundAssertion = new FoundAssertion(new ThrowHandler());
            
            IFlowBuilderService flowBuilderService = new FlowBuilderService();
            IFlowSubscriptionMediator flowSubscriptionMediator = new FlowSubscriptionMediator(_flowRepository, flowBuilderService, bufferMessenger, foundAssertion);

            flowSubscriptionMediator.ConstructAndSubscribe<CurrencyUpdate, CurrencyUpdateError>();
            flowSubscriptionMediator.ConstructAndSubscribe<CurrencyCreation, CurrencyCreationError>();
            flowSubscriptionMediator.ConstructAndSubscribe<SetSkill, SetSkillError>();
            flowSubscriptionMediator.ConstructAndSubscribe<SetHarvestNode, SetHarvestNodeError>();
            flowSubscriptionMediator.ConstructAndSubscribe<SkillUpdateResponse, HarvestNodeUpdateError>();
            flowSubscriptionMediator.ConstructAndSubscribe<NodeCreation, NodeCreationError>();
        }

        [MemberNotNull(nameof(_flowRepository))]
        private static void ValidateRepositoryNotNull()
        {
            ArgumentNullException.ThrowIfNull(_flowRepository);
        }
    }
}
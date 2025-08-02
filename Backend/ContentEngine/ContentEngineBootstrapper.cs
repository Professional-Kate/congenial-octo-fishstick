using ContentEngine.Runtime.ECS;
using ContentEngine.Runtime.Mediator;
using ContentEngine.Runtime.Services;
using ContentEngine.Services;
using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Errors;
using IdelPog.Common.Factories;
using IdelPog.Common.Level;
using IdelPog.Common.Level.Assertions;
using IdelPog.Common.Level.Pipelines;
using IdelPog.Common.Repository;
using IdelPog.Common.Responses;
using IdelPog.Common.Structures;
using IdelPog.Flows.Builder;
using IdelPog.Flows.Types;
using IdelPog.Messaging.Controller;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Listeners.Single;
using IdelPog.Messaging.Orchestration;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace ContentEngine
{
    public static class ContentEngineBootstrapper
    {
        /// <summary>
        /// Creates and adds the <see cref="SetHarvestNode"/> and <see cref="SkillUpdateResponse"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="flowDescriptorDispatcher">Used to dispatch a <see cref="FlowDescriptor"/></param>
        /// <param name="currentResourceProvider">Used together with <see cref="ICurrentResourceProvider"/></param>
        /// <seealso cref="RegisterSetHarvestNode"/>
        public static void RegisterFlows(IBufferManager bufferManager, IDispatchOne<FlowDescriptor> flowDescriptorDispatcher, CurrentResourceProvider currentResourceProvider)
        {
            IFoundAssertion foundAssertion = new FoundAssertion(new ThrowHandler());
            
            IAssetRepository<SkillID, SkillNodeEntity> skillNodeRepository = new AssetRepository<SkillID, SkillNodeEntity>();
            ISkillNodeAccessValidator skillNodeAccessValidator = new SkillNodeAccessValidator(skillNodeRepository, foundAssertion);
            
            RegisterSkillUpdateResponse(bufferManager, flowDescriptorDispatcher, currentResourceProvider, skillNodeAccessValidator);
            RegisterSetHarvestNode(bufferManager, skillNodeRepository, flowDescriptorDispatcher, currentResourceProvider, skillNodeAccessValidator);
        }

        /// <summary>
        /// Registers the <see cref="SkillUpdateResponse"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="flowDescriptorDispatcher">Used to dispatch a <see cref="FlowDescriptor"/></param>
        /// <param name="currentResourceProvider">Used together with <see cref="ICurrentResourceSetter"/></param>
        /// <param name="skillNodeAccessValidator">Used to validate if a skill can access a node</param>
        /// <remarks>
        /// Listens to -> <see cref="SkillUpdateResponse"/>. On Success -> <see cref="HarvestNodeUpdateResponse"/>. On Error -> <see cref="HarvestNodeUpdateError"/>
        /// </remarks>
        private static void RegisterSkillUpdateResponse(IBufferManager bufferManager, IDispatchOne<FlowDescriptor> flowDescriptorDispatcher, ICurrentResourceProvider currentResourceProvider, ISkillNodeAccessValidator skillNodeAccessValidator)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            ILevelAssertion levelAssertion = new LevelAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            ILevelableAssertionPipeline levelableAssertion = new LevelableAssertionPipeline(levelAssertion, objectNullAssertion);
            
            IStateRepository<ResourceID, HarvestNode> harvestNodeRepository = new StateRepository<ResourceID, HarvestNode>();
            ILevelService levelService = new LevelService(levelableAssertion);
            ILevelProgressFactory levelProgressFactory = new LevelProgressFactory();
            IHarvestNodeUpdateResponseFactory responseFactory = new HarvestNodeUpdateResponseFactory(levelProgressFactory);
            
            IDispatchOne<HarvestNodeUpdateResponse> responseDispatcher = new ManagedDispatcher<HarvestNodeUpdateResponse>(bufferManager, objectNullAssertion, collectionAssertion);
            INodeUpdateService nodeUpdateService = new NodeUpdateService(harvestNodeRepository, levelService, responseFactory, foundAssertion);
            ISingleMediator<SkillUpdateResponse> updateMediator = new NodeUpdateMediator(currentResourceProvider, skillNodeAccessValidator, nodeUpdateService, responseDispatcher);
            ISingleController<SkillUpdateResponse> nodeUpdateController = new ManagedSingleController<SkillUpdateResponse>(updateMediator);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<HarvestNodeUpdateError, SkillUpdateResponse> harvestNodeErrorFactory = new HarvestNodeUpdateErrorFactory(baseErrorFactory);
            IDispatchOne<HarvestNodeUpdateError> updateErrorDispatcher = new ManagedDispatcher<HarvestNodeUpdateError>(bufferManager, objectNullAssertion, collectionAssertion);
            
            FlowDescriptor flowDescriptor = new FlowBuilder()
                .ForCommand(typeof(SkillUpdateResponse))
                .SetDispatchMode(BufferMode.SINGLE)
                .SetDescription(typeof(SkillUpdateResponse), typeof(HarvestNodeUpdateResponse), typeof(HarvestNodeUpdateError))
                .WithController(nodeUpdateController)
                .WithErrorDispatcher(updateErrorDispatcher)
                .WithErrorFactory(harvestNodeErrorFactory)
                .Build();
            
            flowDescriptorDispatcher.Dispatch(flowDescriptor);
        }

        /// <summary>
        /// Registers the <see cref="SetHarvestNode"/> flow into the messaging system
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="skillNodeRepository">Used to store all <see cref="HarvestNode"/> models</param>
        /// <param name="flowDescriptorDispatcher">Used to dispatch a <see cref="FlowDescriptor"/></param>
        /// <param name="currentResourceSetter">Used together with <see cref="ICurrentResourceProvider"/></param>
        /// <param name="skillNodeAccessValidator">Used to validate if a skill can access a node</param>
        /// <remarks>
        /// Listens to -> <see cref="SetHarvestNode"/>. On Success -> <see cref="SetHarvestNodeResponse"/>. On Error -> <see cref="SetHarvestNodeError"/>
        /// </remarks>
        private static void RegisterSetHarvestNode(IBufferManager bufferManager, IAssetRepository<SkillID, SkillNodeEntity> skillNodeRepository, IDispatchOne<FlowDescriptor> flowDescriptorDispatcher, ICurrentResourceSetter currentResourceSetter, ISkillNodeAccessValidator skillNodeAccessValidator)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SetHarvestNodeError, SetHarvestNode> setHarvestNodeErrorFactory = new SetHarvestNodeErrorFactory(baseErrorFactory);
            IDispatchOne<SetHarvestNodeError> harvestNodeErrorDispatcher = new ManagedDispatcher<SetHarvestNodeError>(bufferManager, objectNullAssertion, collectionAssertion);

            ResourceComponent stoneResourceComponent = new() { ResourceID = ResourceID.STONE }; 
            ResourceComponent[] resourceComponents = [stoneResourceComponent];
            
            SkillComponent skillComponent = new() { SkillID = SkillID.MINING };
            skillNodeRepository.Add(SkillID.MINING, new SkillNodeEntity(skillComponent, resourceComponents));

            IDispatchOne<SetHarvestNodeResponse> setHarvestNodeResponseDispatcher = new ManagedDispatcher<SetHarvestNodeResponse>(bufferManager, objectNullAssertion, collectionAssertion);
            ISetHarvestNodeResponseFactory nodeChangeResponseFactor = new SetHarvestNodeResponseFactory();
            ISingleMediator<SetHarvestNode> setHarvestNodeMediator = new SetHarvestNodeMediator(skillNodeAccessValidator, currentResourceSetter, setHarvestNodeResponseDispatcher, nodeChangeResponseFactor);
            ISingleController<SetHarvestNode> setHarvestNodeController = new ManagedSingleController<SetHarvestNode>(setHarvestNodeMediator);
            
            FlowDescriptor flowDescriptor = new FlowBuilder()
                .ForCommand(typeof(SetHarvestNode))
                .SetDispatchMode(BufferMode.SINGLE)
                .SetDescription(typeof(SetHarvestNode), typeof(SetHarvestNodeResponse), typeof(SetHarvestNodeError))
                .WithController(setHarvestNodeController)
                .WithErrorDispatcher(harvestNodeErrorDispatcher)
                .WithErrorFactory(setHarvestNodeErrorFactory)
                .Build();
            
            flowDescriptorDispatcher.Dispatch(flowDescriptor);
        }
    }
}
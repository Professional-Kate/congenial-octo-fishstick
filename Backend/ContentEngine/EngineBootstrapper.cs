using ContentEngine.Runtime.ECS;
using ContentEngine.Runtime.Systems;
using ContentEngine.Services;
using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Errors;
using IdelPog.Common.Factories;
using IdelPog.Common.Repository;
using IdelPog.Common.Responses;
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
    public class EngineBootstrapper
    {
        public static void RegisterSetHarvestNode(IBufferManager bufferManager, IDispatchOne<FlowDescriptor> flowDescriptorDispatcher, ICurrentResourceSetter currentResourceSetter)
        {
            IHandler throwHandler = new ThrowHandler();
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SetHarvestNodeError, SetHarvestNode> setHarvestNodeErrorFactory = new SetHarvestNodeErrorFactory(baseErrorFactory);
            IDispatchOne<SetHarvestNodeError> harvestNodeErrorDispatcher = new ManagedDispatcher<SetHarvestNodeError>(bufferManager, objectNullAssertion, collectionAssertion);
            IAssetRepository<SkillID, SkillNodeEntity> skillNodeRepository = new AssetRepository<SkillID, SkillNodeEntity>();

            ResourceComponent stoneResourceComponent = new() { ResourceID = ResourceID.STONE }; 
            ResourceComponent[] resourceComponents = [stoneResourceComponent];
            
            SkillComponent skillComponent = new() { SkillID = SkillID.MINING };
            skillNodeRepository.Add(SkillID.MINING, new SkillNodeEntity(skillComponent, resourceComponents));
            
            IDispatchOne<SetHarvestNodeResponse> setHarvestNodeResponseDispatcher = new ManagedDispatcher<SetHarvestNodeResponse>(bufferManager, objectNullAssertion, collectionAssertion);
            ISetHarvestNodeResponseFactory nodeChangeResponseFactor = new SetHarvestNodeResponseFactory();
            ISingleMediator<SetHarvestNode> setHarvestNodeMediator = new SetHarvestNodeMediator(skillNodeRepository, currentResourceSetter, setHarvestNodeResponseDispatcher, nodeChangeResponseFactor, foundAssertion);
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
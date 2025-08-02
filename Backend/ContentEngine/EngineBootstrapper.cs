using ContentEngine.Runtime.ECS;
using ContentEngine.Runtime.Systems;
using ContentEngine.Services;
using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Errors;
using IdelPog.Common.Factories;
using IdelPog.Common.Repository;
using IdelPog.Common.Responses;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Messenger;
using IdelPog.Messaging.Orchestration;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace ContentEngine
{
    public class EngineBootstrapper
    {
        public void Initialize(IBufferMessenger messenger, IBufferManager bufferManager, ICurrentResourceSetter currentResourceSetter)
        {
            IHandler throwHandler = new ThrowHandler();
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<SetHarvestNodeError, SetHarvestNode> nodeChangeDTOFactory = new SetHarvestNodeErrorFactory(baseErrorFactory);
            IDispatchOne<SetHarvestNodeError> harvestNodeErrorDispatcher = new ManagedDispatcher<SetHarvestNodeError>(bufferManager, objectNullAssertion, collectionAssertion);
            IAssetRepository<SkillID, SkillNodeEntity> skillNodeRepository = new AssetRepository<SkillID, SkillNodeEntity>();

            ResourceComponent stoneResourceComponent = new() { ResourceID = ResourceID.STONE }; 
            ResourceComponent[] resourceComponents = [stoneResourceComponent];
            
            SkillComponent skillComponent = new() { SkillID = SkillID.MINING };
            skillNodeRepository.Add(SkillID.MINING, new SkillNodeEntity(skillComponent, resourceComponents));
            
            IDispatchOne<SetHarvestNodeResponse> setHarvestNodeResponseDispatcher = new ManagedDispatcher<SetHarvestNodeResponse>(bufferManager, objectNullAssertion, collectionAssertion);
            ISetHarvestNodeResponseFactory nodeChangeResponseFactor = new SetHarvestNodeResponseFactory();
            IHarvestNodeAccessSystem harvestNodeAccessSystem = new HarvestNodeAccessSystem(skillNodeRepository, currentResourceSetter, setHarvestNodeResponseDispatcher, nodeChangeResponseFactor, foundAssertion);
            SetHarvestNodeListener harvestNodeListener = new(harvestNodeAccessSystem);
            
            messenger.Subscribe(harvestNodeListener);
        }
    }
}
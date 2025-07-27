using ContentEngine.Runtime.ECS;
using ContentEngine.Runtime.Systems;
using ContentEngine.Services;
using IdelPog.Common.DTO;
using IdelPog.Common.DTO.Factories;
using IdelPog.Common.Enums;
using IdelPog.Common.Repository;
using IdelPog.Messaging.Dispatch;
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

            INodeChangeDTOFactory nodeChangeDTOFactory = new NodeChangeDTOFactory();
            IDispatchOne<ResourceChangeDTO> harvestNodeChangeDispatcher = new ManagedDispatcher<ResourceChangeDTO>(bufferManager, objectNullAssertion, collectionAssertion);
            IAssetRepository<SkillID, SkillNodeEntity> skillNodeRepository = new AssetRepository<SkillID, SkillNodeEntity>();

            ResourceComponent stoneResourceComponent = new() { ResourceID = ResourceID.STONE }; 
            ResourceComponent[] resourceComponents = [stoneResourceComponent];
            
            SkillComponent skillComponent = new() { SkillID = SkillID.MINING };
            skillNodeRepository.Add(SkillID.MINING, new SkillNodeEntity(skillComponent, resourceComponents));
            
            IHarvestNodeAccessSystem harvestNodeAccessSystem = new HarvestNodeAccessSystem(skillNodeRepository, currentResourceSetter, harvestNodeChangeDispatcher, nodeChangeDTOFactory, foundAssertion);
            SetHarvestNodeListener harvestNodeListener = new(harvestNodeAccessSystem);
            
            messenger.Subscribe(harvestNodeListener);
        }
    }
}
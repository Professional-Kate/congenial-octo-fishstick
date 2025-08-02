using ContentEngine.Runtime.ECS;
using ContentEngine.Services;
using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Repository;
using IdelPog.Common.Responses;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Validation.Assertions;

namespace ContentEngine.Runtime.Systems
{
    public class HarvestNodeAccessSystem : IHarvestNodeAccessSystem
    {
        private readonly IAssetRepository<SkillID, SkillNodeEntity> _skillNodeEntityRepository;
        private readonly ICurrentResourceSetter _currentResourceSetter;
        private readonly IDispatchOne<SetHarvestNodeResponse> _harvestNodeDispatcher;
        private readonly ISetHarvestNodeResponseFactory  _nodeChangeResponseFactory;
        private readonly IFoundAssertion _foundAssertion;

        public HarvestNodeAccessSystem(IAssetRepository<SkillID, SkillNodeEntity> skillNodeEntityRepository, ICurrentResourceSetter currentResourceSetter, IDispatchOne<SetHarvestNodeResponse> harvestNodeDispatcher, ISetHarvestNodeResponseFactory nodeChangeResponseFactory, IFoundAssertion foundAssertion)
        {
            _skillNodeEntityRepository = skillNodeEntityRepository;
            _currentResourceSetter = currentResourceSetter;
            _harvestNodeDispatcher = harvestNodeDispatcher;
            _nodeChangeResponseFactory = nodeChangeResponseFactory;
            _foundAssertion = foundAssertion;
        }

        public void UpdateHarvestNode(SetHarvestNode setHarvestNode)
        {
            SkillID skillID = setHarvestNode.SkillID;
            _foundAssertion.AssertFound(skillID, _skillNodeEntityRepository.Contains(setHarvestNode.SkillID));
            
            ResourceID resourceID = setHarvestNode.ResourceID;
            SkillNodeEntity skillNodeEntity = _skillNodeEntityRepository.Get(skillID);
            _foundAssertion.AssertFound(resourceID, skillNodeEntity.Allows(resourceID));
            
            _currentResourceSetter.SetCurrentResource(resourceID);
            _harvestNodeDispatcher.Dispatch(_nodeChangeResponseFactory.Create(setHarvestNode));
        }
    }
}
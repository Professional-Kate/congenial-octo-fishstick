using ContentEngine.Runtime.ECS;
using ContentEngine.Services;
using IdelPog.Common.Commands;
using IdelPog.Common.DTO;
using IdelPog.Common.DTO.Factories;
using IdelPog.Common.Enums;
using IdelPog.Common.Repository;
using IdelPog.Messaging.Dispatch;
using IdelPog.Validation.Assertions;

namespace ContentEngine.Runtime.Systems
{
    public class HarvestNodeAccessSystem : IHarvestNodeAccessSystem
    {
        private readonly IAssetRepository<SkillID, SkillNodeEntity> _skillNodeEntityRepository;
        private readonly ICurrentResourceSetter _currentResourceSetter;
        private readonly IDispatchOne<ResourceChangeDTO> _harvestNodeDTODispatcher;
        private readonly INodeChangeDTOFactory  _nodeChangeDTOFactory;
        private readonly IFoundAssertion _foundAssertion;

        public HarvestNodeAccessSystem(IAssetRepository<SkillID, SkillNodeEntity> skillNodeEntityRepository, ICurrentResourceSetter currentResourceSetter, IDispatchOne<ResourceChangeDTO> harvestNodeDTODispatcher, INodeChangeDTOFactory  nodeChangeDTOFactory, IFoundAssertion foundAssertion)
        {
            _skillNodeEntityRepository = skillNodeEntityRepository;
            _currentResourceSetter = currentResourceSetter;
            _harvestNodeDTODispatcher = harvestNodeDTODispatcher;
            _nodeChangeDTOFactory = nodeChangeDTOFactory;
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
            _harvestNodeDTODispatcher.Dispatch(_nodeChangeDTOFactory.Create(resourceID));
        }
    }
}
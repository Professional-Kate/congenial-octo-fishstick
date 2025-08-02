using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Level;
using IdelPog.Common.Repository;
using IdelPog.Common.Responses;
using IdelPog.Common.Structures;
using IdelPog.Validation.Assertions;

namespace ContentEngine.Runtime.Services
{
    public class NodeUpdateService : INodeUpdateService
    {
        private readonly IStateRepository<ResourceID, HarvestNode> _harvestNodeRepository;
        private readonly ILevelService _levelService;
        private readonly IHarvestNodeUpdateResponseFactory _harvestNodeUpdateResponseFactory;
        private readonly IFoundAssertion _foundAssertion;

        public NodeUpdateService(IStateRepository<ResourceID, HarvestNode> harvestNodeRepository, ILevelService levelService, IHarvestNodeUpdateResponseFactory harvestNodeUpdateResponseFactory, IFoundAssertion foundAssertion)
        {
            _harvestNodeRepository = harvestNodeRepository;
            _levelService = levelService;
            _harvestNodeUpdateResponseFactory = harvestNodeUpdateResponseFactory;
            _foundAssertion = foundAssertion;
        }

        public HarvestNodeUpdateResponse UpdateHarvestNode(ResourceID resourceID)
        {
            _foundAssertion.AssertFound(resourceID, _harvestNodeRepository.Contains(resourceID));
            HarvestNode harvestNode = _harvestNodeRepository.Get(resourceID);

            bool canLevel = _levelService.CanLevel(harvestNode.Levelable);
            if (canLevel)
            {
                _levelService.LevelUp(harvestNode.Levelable);
            }
            
            _harvestNodeRepository.Update(harvestNode.ResourceID, harvestNode);
            
            HarvestNodeUpdateResponse updateResponse = _harvestNodeUpdateResponseFactory.Create(harvestNode, canLevel);
            return updateResponse;
        }
    }
}
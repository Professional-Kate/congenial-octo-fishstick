using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Progression;
using IdelPog.Core.Progression.Experience;
using IdelPog.Core.Progression.Level;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.HarvestNode.Factory.Interface;
using IdelPog.HarvestNode.Runtime.System.Interface;

namespace IdelPog.HarvestNode.Runtime.System
{
    public class NodeUpdateService : INodeUpdateService
    {
        private readonly IStateRepository<ResourceID, Contracts.HarvestNode> _harvestNodeRepository;
        private readonly ILevelService _levelService;
        private readonly IExperienceService _experienceService;
        private readonly INodeUpdateResponseFactory _nodeUpdateResponseFactory;
        private readonly IFoundAssertion _foundAssertion;

        public NodeUpdateService(IStateRepository<ResourceID, Contracts.HarvestNode> harvestNodeRepository, ILevelService levelService, IExperienceService experienceService, INodeUpdateResponseFactory nodeUpdateResponseFactory, IFoundAssertion foundAssertion)
        {
            _harvestNodeRepository = harvestNodeRepository;
            _levelService = levelService;
            _experienceService = experienceService;
            _nodeUpdateResponseFactory = nodeUpdateResponseFactory;
            _foundAssertion = foundAssertion;
        }

        public HarvestNodeUpdateResponse UpdateHarvestNode(ResourceID resourceID)
        {
            _foundAssertion.AssertFound(resourceID, _harvestNodeRepository.Contains(resourceID));
            Contracts.HarvestNode harvestNode = _harvestNodeRepository.Get(resourceID);

            Levelable levelable = harvestNode.Levelable;
            _experienceService.AddExperience(levelable);
            
            bool canLevel = _levelService.CanLevel(levelable);
            if (canLevel)
            {
                _levelService.LevelUp(levelable);
            }
            
            _harvestNodeRepository.Update(harvestNode.ResourceID, harvestNode);
            
            HarvestNodeUpdateResponse updateResponse = _nodeUpdateResponseFactory.Create(harvestNode, canLevel);
            return updateResponse;
        }
    }
}
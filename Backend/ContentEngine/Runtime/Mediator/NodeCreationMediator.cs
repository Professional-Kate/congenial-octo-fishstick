using ContentEngine.Runtime.ECS;
using ContentEngine.Runtime.Factory.Interfaces;
using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Repository;
using IdelPog.Common.Responses;
using IdelPog.Common.Structures;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Validation.Assertions;

namespace ContentEngine.Runtime.Mediator
{
    public class NodeCreationMediator : IBatchMediator<NodeCreation>
    {
        private readonly IStateRepository<ResourceID, HarvestNode> _harvestNodeRepository;
        private readonly IAssetRepository<SkillID, SkillNodeEntity> _skillNodeEntityRepository;
        private readonly ISkillNodeEntityFactory  _skillNodeEntityFactory;
        private readonly IHarvestNodeFactory  _harvestNodeFactory;
        private readonly INodeCreationResponseFactory  _nodeCreationResponseFactory;
        private readonly IDispatchOne<NodeCreationResponse> _nodeCreationResponseDispatcher;
        private readonly IUniqueAssertion _uniqueAssertion;

        public NodeCreationMediator(IStateRepository<ResourceID, HarvestNode> harvestNodeRepository, IAssetRepository<SkillID, SkillNodeEntity> skillNodeEntityRepository, ISkillNodeEntityFactory skillNodeEntityFactory, IHarvestNodeFactory harvestNodeFactory, INodeCreationResponseFactory  nodeCreationResponseFactory, IDispatchOne<NodeCreationResponse> nodeCreationResponseDispatcher, IUniqueAssertion uniqueAssertion)
        {
            _harvestNodeRepository = harvestNodeRepository;
            _skillNodeEntityRepository = skillNodeEntityRepository;
            _skillNodeEntityFactory = skillNodeEntityFactory;
            _harvestNodeFactory = harvestNodeFactory;
            _nodeCreationResponseFactory = nodeCreationResponseFactory;
            _nodeCreationResponseDispatcher = nodeCreationResponseDispatcher;
            _uniqueAssertion = uniqueAssertion;
        }

        public void HandleMessages(IReadOnlyList<NodeCreation> nodeCreations)
        {
            foreach (NodeCreation nodeCreation in nodeCreations)
            {
                foreach (ResourceID nodeCreationResourceID in nodeCreation.ResourceIDs)
                {
                    _uniqueAssertion.AssertUnique(nodeCreationResourceID, _harvestNodeRepository.Contains(nodeCreationResourceID));
                }

                _uniqueAssertion.AssertUnique(nodeCreation.LinkedSkill, _skillNodeEntityRepository.Contains(nodeCreation.LinkedSkill));

                CreateHarvestNodes(nodeCreation);
                CreateSkillNodeEntity(nodeCreation);
            }
            
            _nodeCreationResponseDispatcher.Dispatch(_nodeCreationResponseFactory.Create(nodeCreations.ToArray()));
        }
        
        private void CreateHarvestNodes(NodeCreation nodeCreation)
        {
            foreach (ResourceID resourceID in nodeCreation.ResourceIDs)
            { 
                _harvestNodeRepository.Add(resourceID, _harvestNodeFactory.Create(resourceID));
            }
        }
        
        private void CreateSkillNodeEntity(NodeCreation nodeCreation)
        {
            SkillNodeEntity skillNodeEntity = _skillNodeEntityFactory.Create(nodeCreation.LinkedSkill, nodeCreation.ResourceIDs);
            _skillNodeEntityRepository.Add(nodeCreation.LinkedSkill, skillNodeEntity);
        }
    }
}
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.HarvestNode.Contracts;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Response;
using IdelPog.HarvestNode.Factory.Interface;
using IdelPog.HarvestNode.Runtime.ECS;
using IdelPog.HarvestNode.Runtime.Factory.Interface;

namespace IdelPog.HarvestNode.Runtime.Mediator
{
    public sealed class NodeCreationMediator : IBatchMediator<HarvestNodeCreation>
    {
        private readonly IStateRepository<ResourceID, Contracts.HarvestNode> _harvestNodeRepository;
        private readonly IAssetRepository<SkillID, SkillNodeEntity> _skillNodeEntityRepository;
        private readonly ISkillNodeEntityFactory  _skillNodeEntityFactory;
        private readonly IHarvestNodeFactory  _harvestNodeFactory;
        private readonly INodeCreationResponseFactory  _nodeCreationResponseFactory;
        private readonly IDispatchMany<HarvestNodeCreationResponse> _nodeCreationResponseDispatcher;
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public NodeCreationMediator(IStateRepository<ResourceID, Contracts.HarvestNode> harvestNodeRepository, IAssetRepository<SkillID, SkillNodeEntity> skillNodeEntityRepository, ISkillNodeEntityFactory skillNodeEntityFactory, IHarvestNodeFactory harvestNodeFactory, INodeCreationResponseFactory  nodeCreationResponseFactory, IDispatchMany<HarvestNodeCreationResponse> nodeCreationResponseDispatcher, IUniqueAssertion uniqueAssertion, ICollectionAssertion collectionAssertion)
        {
            _harvestNodeRepository = harvestNodeRepository;
            _skillNodeEntityRepository = skillNodeEntityRepository;
            _skillNodeEntityFactory = skillNodeEntityFactory;
            _harvestNodeFactory = harvestNodeFactory;
            _nodeCreationResponseFactory = nodeCreationResponseFactory;
            _nodeCreationResponseDispatcher = nodeCreationResponseDispatcher;
            _uniqueAssertion = uniqueAssertion;
            _collectionAssertion = collectionAssertion;
        }

        public void HandleMessages(IReadOnlyList<HarvestNodeCreation> nodeCreations)
        {
            _collectionAssertion.AssertHasElements(nodeCreations);
            
            foreach (HarvestNodeCreation nodeCreation in nodeCreations)
            {
                _collectionAssertion.AssertHasElements(nodeCreation.ReadOnlyHarvestNodes);
                
                foreach (ReadOnlyHarvestNode readOnlyHarvestNode in nodeCreation.ReadOnlyHarvestNodes)
                {
                    _uniqueAssertion.AssertUnique(readOnlyHarvestNode.ResourceID, _harvestNodeRepository.Contains(readOnlyHarvestNode.ResourceID));
                }

                _uniqueAssertion.AssertUnique(nodeCreation.LinkedSkill, _skillNodeEntityRepository.Contains(nodeCreation.LinkedSkill));

                CreateHarvestNodes(nodeCreation);
                CreateSkillNodeEntity(nodeCreation);
            }
            
            _nodeCreationResponseDispatcher.Dispatch(_nodeCreationResponseFactory.Create(nodeCreations.ToArray()));
        }
        
        private void CreateHarvestNodes(HarvestNodeCreation harvestNodeCreation)
        {
            foreach (ReadOnlyHarvestNode readOnlyHarvestNode in harvestNodeCreation.ReadOnlyHarvestNodes)
            { 
                _harvestNodeRepository.Add(readOnlyHarvestNode.ResourceID, _harvestNodeFactory.Create(readOnlyHarvestNode));
            }
        }
        
        private void CreateSkillNodeEntity(HarvestNodeCreation harvestNodeCreation)
        {
            SkillNodeEntity skillNodeEntity = _skillNodeEntityFactory.Create(harvestNodeCreation.LinkedSkill, harvestNodeCreation.ReadOnlyHarvestNodes);
            _skillNodeEntityRepository.Add(harvestNodeCreation.LinkedSkill, skillNodeEntity);
        }
    }
}
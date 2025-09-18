using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.HarvestNode.Factory.Interface;
using IdelPog.HarvestNode.Runtime.ECS;
using IdelPog.HarvestNode.Runtime.Factory.Interfaces;

namespace IdelPog.HarvestNode.Runtime.Mediator
{
    public class NodeCreationMediator : IBatchMediator<NodeCreation>
    {
        private readonly IStateRepository<ItemID, Contracts.HarvestNode> _harvestNodeRepository;
        private readonly IAssetRepository<SkillID, SkillNodeEntity> _skillNodeEntityRepository;
        private readonly ISkillNodeEntityFactory  _skillNodeEntityFactory;
        private readonly IHarvestNodeFactory  _harvestNodeFactory;
        private readonly INodeCreationResponseFactory  _nodeCreationResponseFactory;
        private readonly IDispatchOne<NodeCreationResponse> _nodeCreationResponseDispatcher;
        private readonly IUniqueAssertion _uniqueAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public NodeCreationMediator(IStateRepository<ItemID, Contracts.HarvestNode> harvestNodeRepository, IAssetRepository<SkillID, SkillNodeEntity> skillNodeEntityRepository, ISkillNodeEntityFactory skillNodeEntityFactory, IHarvestNodeFactory harvestNodeFactory, INodeCreationResponseFactory  nodeCreationResponseFactory, IDispatchOne<NodeCreationResponse> nodeCreationResponseDispatcher, IUniqueAssertion uniqueAssertion, ICollectionAssertion collectionAssertion)
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

        public void HandleMessages(IReadOnlyList<NodeCreation> nodeCreations)
        {
            _collectionAssertion.AssertHasElements(nodeCreations);
            
            foreach (NodeCreation nodeCreation in nodeCreations)
            {
                _collectionAssertion.AssertHasElements(nodeCreation.ItemIDs);
                
                foreach (ItemID nodeCreationResourceID in nodeCreation.ItemIDs)
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
            foreach (ItemID itemID in nodeCreation.ItemIDs)
            { 
                _harvestNodeRepository.Add(itemID, _harvestNodeFactory.Create(itemID));
                Console.WriteLine(itemID);
            }
        }
        
        private void CreateSkillNodeEntity(NodeCreation nodeCreation)
        {
            SkillNodeEntity skillNodeEntity = _skillNodeEntityFactory.Create(nodeCreation.LinkedSkill, nodeCreation.ItemIDs);
            _skillNodeEntityRepository.Add(nodeCreation.LinkedSkill, skillNodeEntity);
        }
    }
}
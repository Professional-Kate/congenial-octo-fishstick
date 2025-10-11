using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.HarvestNode.Assertion.Interface;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.Progression.Runtime.System.Interface;

namespace IdelPog.HarvestNode.Runtime.Mediator
{
    public sealed class NodeUpdateMediator : IBatchMediator<HarvestNodeUpdate>
    {
        private readonly IStateRepository<ResourceID, Contracts.HarvestNode> _harvestNodeRepository;
        private readonly ISkillNodeAccessValidator _skillNodeAccessValidator;
        private readonly INodeUpdateService _nodeUpdateService;
        private readonly IHarvestNodeLootService _lootService;
        private readonly IDispatchMany<HarvestNodeUpdateResponse> _updateResponseDispatcher;
        private readonly IDispatchMany<InventoryUpdate> _inventoryUpdateDispatcher;
        private readonly IEntityUnlockChecker<SkillID, HarvestNodeUnlockResponse> _unlockChecker;
        private readonly INodeUnlockedAssertion _nodeUnlockedAssertion;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IFoundAssertion _foundAssertion;

        public NodeUpdateMediator(IStateRepository<ResourceID, Contracts.HarvestNode> harvestNodeRepository, ISkillNodeAccessValidator skillNodeAccessValidator,
            IEntityUnlockChecker<SkillID, HarvestNodeUnlockResponse> unlockChecker, INodeUpdateService nodeUpdateService, IHarvestNodeLootService lootService,
            IDispatchMany<HarvestNodeUpdateResponse> updateResponseDispatcher, IDispatchMany<InventoryUpdate> inventoryUpdateDispatcher,
            INodeUnlockedAssertion nodeUnlockedAssertion, ICollectionAssertion collectionAssertion, IFoundAssertion foundAssertion)
        {
            _skillNodeAccessValidator = skillNodeAccessValidator;
            _nodeUpdateService = nodeUpdateService;
            _updateResponseDispatcher = updateResponseDispatcher;
            _lootService = lootService;
            _unlockChecker = unlockChecker;
            _nodeUnlockedAssertion = nodeUnlockedAssertion;
            _collectionAssertion = collectionAssertion;
            _harvestNodeRepository = harvestNodeRepository;
            _inventoryUpdateDispatcher = inventoryUpdateDispatcher;
            _foundAssertion = foundAssertion;
        }

        public void HandleMessages(IReadOnlyList<HarvestNodeUpdate> messages)
        {
            _collectionAssertion.AssertHasElements(messages);

            List<InventoryUpdate> inventoryUpdates = [];
            HarvestNodeUpdateResponse[] responses = new HarvestNodeUpdateResponse[messages.Count];
            
            for (int i = 0; i < messages.Count; i++)
            {
                HarvestNodeUpdate harvestNodeUpdate = messages[i];
                ValidateUnlocked(harvestNodeUpdate);
                
                ResourceID resourceID = harvestNodeUpdate.ResourceID;
                responses[i] = _nodeUpdateService.UpdateHarvestNode(resourceID);

                inventoryUpdates.AddRange(GenerateInventoryUpdates(resourceID));
            }

            _updateResponseDispatcher.Dispatch(responses);
            DispatchInventoryUpdates(inventoryUpdates);
        }

        private void ValidateUnlocked(HarvestNodeUpdate harvestNodeUpdate)
        {
            SkillID skillID = harvestNodeUpdate.SkillID;
            ResourceID resourceID = harvestNodeUpdate.ResourceID;
            
            _skillNodeAccessValidator.AssertSkillAllows(skillID, resourceID);

            bool isUnlocked = _unlockChecker.IsUnlocked(skillID, component => component.OnUnlockCommand.ResourceID == resourceID);
            _nodeUnlockedAssertion.AssertNodeIsUnlocked(isUnlocked, harvestNodeUpdate);
        }

        private List<InventoryUpdate> GenerateInventoryUpdates(ResourceID resourceID)
        {
            _foundAssertion.AssertFound(resourceID, _harvestNodeRepository.Contains(resourceID));

            List<InventoryUpdate> inventoryUpdates = [];
            
            Contracts.HarvestNode harvestNode = _harvestNodeRepository.Get(resourceID);
            inventoryUpdates.AddRange(_lootService.GenerateInventoryUpdates(harvestNode));

            return inventoryUpdates;
        }

        private void DispatchInventoryUpdates(List<InventoryUpdate> inventoryUpdates)
        {
            if (inventoryUpdates.Count == 0)
            {
                return;
            }
            
            _inventoryUpdateDispatcher.Dispatch(inventoryUpdates);
        }
    }
}
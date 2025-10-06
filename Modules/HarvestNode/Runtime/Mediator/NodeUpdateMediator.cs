using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.HarvestNode.Assertion.Interface;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.Loot.Service.Interface;
using IdelPog.Progression.Runtime.System.Interface;

namespace IdelPog.HarvestNode.Runtime.Mediator
{
    public sealed class NodeUpdateMediator : IBatchMediator<HarvestNodeUpdate>
    {
        private readonly ISkillNodeAccessValidator _skillNodeAccessValidator;
        private readonly INodeUpdateService _nodeUpdateService;
        private readonly IDispatchMany<HarvestNodeUpdateResponse> _updateResponseDispatcher;
        private readonly ILootService<ItemID> _lootService;
        private readonly IEntityUnlockChecker<SkillID, HarvestNodeUnlockResponse> _unlockChecker;
        private readonly INodeUnlockedAssertion _nodeUnlockedAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public NodeUpdateMediator(ISkillNodeAccessValidator skillNodeAccessValidator, INodeUpdateService nodeUpdateService, IDispatchMany<HarvestNodeUpdateResponse> updateResponseDispatcher, ILootService<ItemID> lootService, IEntityUnlockChecker<SkillID, HarvestNodeUnlockResponse> unlockChecker, INodeUnlockedAssertion nodeUnlockedAssertion, ICollectionAssertion collectionAssertion)
        {
            _skillNodeAccessValidator = skillNodeAccessValidator;
            _nodeUpdateService = nodeUpdateService;
            _updateResponseDispatcher = updateResponseDispatcher;
            _lootService = lootService;
            _unlockChecker = unlockChecker;
            _nodeUnlockedAssertion = nodeUnlockedAssertion;
            _collectionAssertion = collectionAssertion;
        }

        public void HandleMessages(IReadOnlyList<HarvestNodeUpdate> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            HarvestNodeUpdateResponse[] responses = new HarvestNodeUpdateResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                HarvestNodeUpdate harvestNodeUpdate = messages[i];
                
                SkillID skillID = harvestNodeUpdate.SkillID;
                ItemID harvestTarget = harvestNodeUpdate.ItemID;
                _skillNodeAccessValidator.AssertSkillAllows(skillID, harvestTarget);

                bool isUnlocked = _unlockChecker.IsUnlocked(skillID, component => component.OnUnlockCommand.ItemID == harvestTarget);
                _nodeUnlockedAssertion.AssertNodeIsUnlocked(isUnlocked, harvestNodeUpdate);
            
                HarvestNodeUpdateResponse response = _nodeUpdateService.UpdateHarvestNode(harvestTarget);
                responses[i] = response;
            
                _lootService.GenerateItemID(harvestTarget);
            }
            
            _updateResponseDispatcher.Dispatch(responses);
        }
    }
}
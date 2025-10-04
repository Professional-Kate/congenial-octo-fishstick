using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.Loot.Service.Interface;

namespace IdelPog.HarvestNode.Runtime.Mediator
{
    public class NodeUpdateMediator : IBatchMediator<HarvestNodeUpdate>
    {
        private readonly ISkillNodeAccessValidator _skillNodeAccessValidator;
        private readonly INodeUpdateService _nodeUpdateService;
        private readonly IDispatchMany<HarvestNodeUpdateResponse> _updateResponseDispatcher;
        private readonly ILootService<ItemID> _lootService;

        public NodeUpdateMediator(ISkillNodeAccessValidator skillNodeAccessValidator, INodeUpdateService nodeUpdateService, IDispatchMany<HarvestNodeUpdateResponse> updateResponseDispatcher, ILootService<ItemID> lootService)
        {
            _skillNodeAccessValidator = skillNodeAccessValidator;
            _nodeUpdateService = nodeUpdateService;
            _updateResponseDispatcher = updateResponseDispatcher;
            _lootService = lootService;
        }

        public void HandleMessages(IReadOnlyList<HarvestNodeUpdate> messages)
        {
            HarvestNodeUpdateResponse[] responses = new HarvestNodeUpdateResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                HarvestNodeUpdate harvestNodeUpdate = messages[i];
                
                SkillID skillID = harvestNodeUpdate.SkillID;
                ItemID harvestTarget = harvestNodeUpdate.ItemID;
                _skillNodeAccessValidator.AssertSkillAllows(skillID, harvestTarget);
            
                HarvestNodeUpdateResponse response = _nodeUpdateService.UpdateHarvestNode(harvestTarget);
                responses[i] = response;
            
                _lootService.DispatchInventoryUpdates(harvestTarget);
            }
            
            _updateResponseDispatcher.Dispatch(responses);
        }
    }
}
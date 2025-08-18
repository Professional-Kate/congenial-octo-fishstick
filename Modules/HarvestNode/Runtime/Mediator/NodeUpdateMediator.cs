using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.HarvestNode.Services;
using IdelPog.Loot.Service.Interface;

namespace IdelPog.HarvestNode.Runtime.Mediator
{
    public class NodeUpdateMediator : ISingleMediator<SkillUpdateResponse>
    {
        private readonly ICurrentHarvestTargetProvider  _currentHarvestTargetProvider;
        private readonly ISkillNodeAccessValidator _skillNodeAccessValidator;
        private readonly INodeUpdateService _nodeUpdateService;
        private readonly IDispatchOne<HarvestNodeUpdateResponse> _updateResponseDispatcher;
        private readonly ILootService _lootService;

        public NodeUpdateMediator(ICurrentHarvestTargetProvider currentHarvestTargetProvider, ISkillNodeAccessValidator skillNodeAccessValidator, INodeUpdateService nodeUpdateService, IDispatchOne<HarvestNodeUpdateResponse> updateResponseDispatcher, ILootService lootService)
        {
            _currentHarvestTargetProvider = currentHarvestTargetProvider;
            _skillNodeAccessValidator = skillNodeAccessValidator;
            _nodeUpdateService = nodeUpdateService;
            _updateResponseDispatcher = updateResponseDispatcher;
            _lootService = lootService;
        }

        public void HandleMessage(SkillUpdateResponse skillUpdateResponse)
        {
            SkillID skillID = skillUpdateResponse.SkillID;
            ItemID harvestTarget = _currentHarvestTargetProvider.GetCurrentHarvestTarget();
            _skillNodeAccessValidator.AssertSkillAllows(skillID, harvestTarget);
            
            HarvestNodeUpdateResponse response = _nodeUpdateService.UpdateHarvestNode(harvestTarget);
            
            _lootService.DispatchInventoryUpdates(harvestTarget);
            _updateResponseDispatcher.Dispatch(response);
        }
    }
}
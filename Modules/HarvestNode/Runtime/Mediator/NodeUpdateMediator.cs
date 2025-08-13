using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.HarvestNode.Services;

namespace IdelPog.HarvestNode.Runtime.Mediator
{
    public class NodeUpdateMediator : ISingleMediator<SkillUpdateResponse>
    {
        private readonly ICurrentResourceProvider  _currentResourceProvider;
        private readonly ISkillNodeAccessValidator _skillNodeAccessValidator;
        private readonly INodeUpdateService _nodeUpdateService;
        private readonly IDispatchOne<HarvestNodeUpdateResponse> _updateResponseDispatcher;

        public NodeUpdateMediator(ICurrentResourceProvider currentResourceProvider, ISkillNodeAccessValidator skillNodeAccessValidator, INodeUpdateService nodeUpdateService, IDispatchOne<HarvestNodeUpdateResponse> updateResponseDispatcher)
        {
            _currentResourceProvider = currentResourceProvider;
            _skillNodeAccessValidator = skillNodeAccessValidator;
            _nodeUpdateService = nodeUpdateService;
            _updateResponseDispatcher = updateResponseDispatcher;
        }

        public void HandleMessage(SkillUpdateResponse skillUpdateResponse)
        {
            SkillID skillID = skillUpdateResponse.SkillID;
            ResourceID currentResource = _currentResourceProvider.GetCurrentResource();
            _skillNodeAccessValidator.AssertSkillAllows(skillID, currentResource);
            
            HarvestNodeUpdateResponse response = _nodeUpdateService.UpdateHarvestNode(currentResource);
            _updateResponseDispatcher.Dispatch(response);
        }
    }
}
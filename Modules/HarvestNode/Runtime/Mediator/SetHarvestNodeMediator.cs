using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Single;
using IdelPog.HarvestNode.Factory.Interface;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.HarvestNode.Services;

namespace IdelPog.HarvestNode.Runtime.Mediator
{
    public class SetHarvestNodeMediator : ISingleMediator<SetHarvestNode>
    {
        private readonly ISkillNodeAccessValidator _skillNodeAccessValidator;
        private readonly ICurrentResourceSetter _currentResourceSetter;
        private readonly IDispatchOne<SetHarvestNodeResponse> _harvestNodeDispatcher;
        private readonly ISetNodeResponseFactory _nodeChangeResponseFactory;

        public SetHarvestNodeMediator(ISkillNodeAccessValidator skillNodeAccessValidator, ICurrentResourceSetter currentResourceSetter,
            IDispatchOne<SetHarvestNodeResponse> harvestNodeDispatcher, ISetNodeResponseFactory nodeChangeResponseFactory)
        {
            _skillNodeAccessValidator = skillNodeAccessValidator;
            _currentResourceSetter = currentResourceSetter;
            _harvestNodeDispatcher = harvestNodeDispatcher;
            _nodeChangeResponseFactory = nodeChangeResponseFactory;
        }

        public void HandleMessage(SetHarvestNode setHarvestNode)
        {
            SkillID skillID = setHarvestNode.SkillID;
            ResourceID resourceID = setHarvestNode.ResourceID;
            _skillNodeAccessValidator.AssertSkillAllows(skillID, resourceID);

            _currentResourceSetter.SetCurrentResource(resourceID);
            _harvestNodeDispatcher.Dispatch(_nodeChangeResponseFactory.Create(setHarvestNode));
        }
    }
}
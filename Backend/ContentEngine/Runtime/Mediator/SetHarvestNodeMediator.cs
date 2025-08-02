using ContentEngine.Runtime.Services;
using ContentEngine.Services;
using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Common.Responses;
using IdelPog.Messaging.Dispatch.Single;
using IdelPog.Messaging.Listeners.Single;

namespace ContentEngine.Runtime.Mediator
{
    public class SetHarvestNodeMediator : ISingleMediator<SetHarvestNode>
    {
        private readonly ISkillNodeAccessValidator _skillNodeAccessValidator;
        private readonly ICurrentResourceSetter _currentResourceSetter;
        private readonly IDispatchOne<SetHarvestNodeResponse> _harvestNodeDispatcher;
        private readonly ISetHarvestNodeResponseFactory _nodeChangeResponseFactory;

        public SetHarvestNodeMediator(ISkillNodeAccessValidator skillNodeAccessValidator, ICurrentResourceSetter currentResourceSetter,
            IDispatchOne<SetHarvestNodeResponse> harvestNodeDispatcher, ISetHarvestNodeResponseFactory nodeChangeResponseFactory)
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
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
        private readonly ICurrentHarvestTargetSetter _currentHarvestTargetSetter;
        private readonly IDispatchOne<SetHarvestNodeResponse> _harvestNodeDispatcher;
        private readonly ISetNodeResponseFactory _nodeChangeResponseFactory;

        public SetHarvestNodeMediator(ISkillNodeAccessValidator skillNodeAccessValidator, ICurrentHarvestTargetSetter currentHarvestTargetSetter,
            IDispatchOne<SetHarvestNodeResponse> harvestNodeDispatcher, ISetNodeResponseFactory nodeChangeResponseFactory)
        {
            _skillNodeAccessValidator = skillNodeAccessValidator;
            _currentHarvestTargetSetter = currentHarvestTargetSetter;
            _harvestNodeDispatcher = harvestNodeDispatcher;
            _nodeChangeResponseFactory = nodeChangeResponseFactory;
        }

        public void HandleMessage(SetHarvestNode setHarvestNode)
        {
            SkillID skillID = setHarvestNode.SkillID;
            ItemID itemID = setHarvestNode.ItemID;
            _skillNodeAccessValidator.AssertSkillAllows(skillID, itemID);

            _currentHarvestTargetSetter.SetCurrentResource(itemID);
            _harvestNodeDispatcher.Dispatch(_nodeChangeResponseFactory.Create(setHarvestNode));
        }
    }
}
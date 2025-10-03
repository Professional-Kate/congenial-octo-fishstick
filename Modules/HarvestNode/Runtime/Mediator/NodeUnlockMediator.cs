using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Progression.Runtime.System.Interface;

namespace IdelPog.HarvestNode.Runtime.Mediator
{
    public sealed class NodeUnlockMediator : IBatchMediator<HarvestNodeUnlock>
    {
        private readonly IEntityUnlockerService<SkillID, HarvestNodeUnlockResponse> _entityUnlockerService;
        private readonly IDispatchMany<HarvestNodeUnlockResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;

        public NodeUnlockMediator(IEntityUnlockerService<SkillID, HarvestNodeUnlockResponse> entityUnlockerService, IDispatchMany<HarvestNodeUnlockResponse> responseDispatcher, ICollectionAssertion collectionAssertion)
        {
            _entityUnlockerService = entityUnlockerService;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
        }

        public void HandleMessages(IReadOnlyList<HarvestNodeUnlock> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            List<HarvestNodeUnlockResponse> responses = [];

            foreach (HarvestNodeUnlock harvestNodeUnlock in messages)
            {
                if (_entityUnlockerService.CanUnlock(harvestNodeUnlock.SkillID, harvestNodeUnlock.SkillLevel) == false)
                {
                    continue;
                }

                IEnumerable<HarvestNodeUnlockResponse> responsesEnumerable = _entityUnlockerService.UnlockAllAvailable(harvestNodeUnlock.SkillID, harvestNodeUnlock.SkillLevel);
                responses.AddRange(responsesEnumerable);
            }

            if (responses.Count == 0)
            {
                return;
            }
            
            _responseDispatcher.Dispatch(responses);
        }
    }
}
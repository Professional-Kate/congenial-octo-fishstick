using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Progression.Contracts;
using IdelPog.Progression.Runtime.ECS.System.Interface;

namespace IdelPog.Progression.Runtime.ECS.Mediator
{
    public sealed class NodeUnlockMediator : IBatchMediator<HarvestNodeUnlock>
    {
        private readonly INodeUnlockerService _nodeUnlockerService;
        private readonly IDispatchMany<HarvestNodeUnlockResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;

        public NodeUnlockMediator(INodeUnlockerService nodeUnlockerService, IDispatchMany<HarvestNodeUnlockResponse> responseDispatcher, ICollectionAssertion collectionAssertion)
        {
            _nodeUnlockerService = nodeUnlockerService;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
        }

        public void HandleMessages(IReadOnlyList<HarvestNodeUnlock> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            List<HarvestNodeUnlockResponse> responses = [];

            foreach (HarvestNodeUnlock harvestNodeUnlock in messages)
            {
                if (_nodeUnlockerService.CanUnlock(harvestNodeUnlock) == false)
                {
                    continue;
                }

                HarvestNodeUnlockResponse response = _nodeUnlockerService.Unlock(harvestNodeUnlock);
                responses.Add(response);
            }

            if (responses.Count == 0)
            {
                return;
            }
            
            _responseDispatcher.Dispatch(responses);
        }
    }
}
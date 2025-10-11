using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.HarvestNode.Runtime.System.Interface;

namespace IdelPog.HarvestNode.Runtime.Mediator
{
    public sealed class NodeLootCreationMediator : IBatchMediator<HarvestNodeLootCreation>
    {
        private readonly ILootTableService<ResourceID> _lootTableService;
        private readonly IGrantPolicyService<ResourceID> _grantPolicyService;
        private readonly IDispatchMany<HarvestNodeLootCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;

        public NodeLootCreationMediator(ILootTableService<ResourceID> lootTableService, IGrantPolicyService<ResourceID> grantPolicyService, IDispatchMany<HarvestNodeLootCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion)
        {
            _lootTableService = lootTableService;
            _grantPolicyService = grantPolicyService;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
        }

        public void HandleMessages(IReadOnlyList<HarvestNodeLootCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);

            HarvestNodeLootCreationResponse[] responses = new HarvestNodeLootCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                HarvestNodeLootCreation creation = messages[i];
                
                _collectionAssertion.AssertHasElements(creation.LootTableEntries);
                
                _lootTableService.CreateLootTable(creation.LootTableEntries, creation.ResourceID);
                _grantPolicyService.CreateGrantPolicy(creation.GrantPolicyEntry, creation.ResourceID);
                
                HarvestNodeLootCreationResponse response = new() { ResourceID = creation.ResourceID, LootTableEntries = creation.LootTableEntries };
                responses[i] = response;
            }
            
            _responseDispatcher.Dispatch(responses);
        }
    }
}
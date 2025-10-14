using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Response;
using IdelPog.HarvestNode.Runtime.System.Interface;

namespace IdelPog.HarvestNode.Runtime.Mediator
{
    public sealed class ResourceLootCreationMediator : IBatchMediator<ResourceLootCreation>
    {
        private readonly ILootTableService<ResourceID> _lootTableService;
        private readonly IGrantPolicyService<ResourceID> _grantPolicyService;
        private readonly IDispatchMany<ResourceLootCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;

        public ResourceLootCreationMediator(ILootTableService<ResourceID> lootTableService, IGrantPolicyService<ResourceID> grantPolicyService, IDispatchMany<ResourceLootCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion)
        {
            _lootTableService = lootTableService;
            _grantPolicyService = grantPolicyService;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
        }

        public void HandleMessages(IReadOnlyList<ResourceLootCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);

            ResourceLootCreationResponse[] responses = new ResourceLootCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                ResourceLootCreation creation = messages[i];
                
                _collectionAssertion.AssertHasElements(creation.LootTableEntries);
                
                _lootTableService.CreateLootTable(creation.LootTableEntries, creation.ResourceID);
                _grantPolicyService.CreateGrantPolicy(creation.GrantPolicyEntry, creation.ResourceID);
                
                ResourceLootCreationResponse response = new() { ResourceID = creation.ResourceID, LootTableEntries = creation.LootTableEntries };
                responses[i] = response;
            }
            
            _responseDispatcher.Dispatch(responses);
        }
    }
}
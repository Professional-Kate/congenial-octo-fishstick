using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.HarvestNode.Runtime.System.Interface;

namespace IdelPog.HarvestNode.Runtime.Mediator
{
    public sealed class LocationLootCreationMediator : IBatchMediator<LocationLootCreation>
    {
        private readonly ILootTableService<LocationID> _lootTableService;
        private readonly IGrantPolicyService<LocationID> _grantPolicyService;
        private readonly IDispatchMany<LocationLootCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;

        public LocationLootCreationMediator(ILootTableService<LocationID> lootTableService, IGrantPolicyService<LocationID> grantPolicyService, IDispatchMany<LocationLootCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion)
        {
            _lootTableService = lootTableService;
            _grantPolicyService = grantPolicyService;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
        }

        public void HandleMessages(IReadOnlyList<LocationLootCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);

            LocationLootCreationResponse[] responses = new LocationLootCreationResponse[messages.Count];
            for (var i = 0; i < messages.Count; i++)
            {
                LocationLootCreation creation = messages[i];
                
                _collectionAssertion.AssertHasElements(creation.LootTableEntries);
                
                _lootTableService.CreateLootTable(creation.LootTableEntries, creation.LocationID);
                _grantPolicyService.CreateGrantPolicy(creation.GrantPolicyEntry, creation.LocationID);
                
                LocationLootCreationResponse response = new() { ResourceID = creation.ResourceID, LocationID = creation.LocationID, LootTableEntries = creation.LootTableEntries };
                responses[i] = response;
            }
            
            _responseDispatcher.Dispatch(responses);
        }
    }
}
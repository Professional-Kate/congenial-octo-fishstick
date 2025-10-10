using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.HarvestNode.Runtime.Factory.Interface;
using IdelPog.Loot.Policy;
using IdelPog.Loot.Random;
using IdelPog.Loot.Table;

namespace IdelPog.HarvestNode.Runtime.Mediator
{
    public sealed class NodeLootGenerationMediator : IBatchMediator<HarvestNodeLootCreation>
    {
        private readonly IAssetRepository<ItemID, ILootTable> _lootTableRepository;
        private readonly IAssetRepository<ItemID, IGrantPolicy> _grantPolicyRepository;
        private readonly IWeightedLootTableFactory _lootTableFactory;
        private readonly IWeightedPolicyFactory _policyFactory;
        private readonly IDispatchMany<HarvestNodeLootCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;

        public NodeLootGenerationMediator(IAssetRepository<ItemID, ILootTable> lootTableRepository, IAssetRepository<ItemID, IGrantPolicy> grantPolicyRepository, IWeightedLootTableFactory lootTableFactory, IWeightedPolicyFactory policyFactory, IDispatchMany<HarvestNodeLootCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion)
        {
            _lootTableRepository = lootTableRepository;
            _grantPolicyRepository = grantPolicyRepository;
            _lootTableFactory = lootTableFactory;
            _policyFactory = policyFactory;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
        }

        public void HandleMessages(IReadOnlyList<HarvestNodeLootCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);

            HarvestNodeLootCreationResponse[] responses = new HarvestNodeLootCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                HarvestNodeLootCreation creation = messages[i];
                
                _uniqueAssertion.AssertUnique(creation.ItemID, _lootTableRepository.Contains(creation.ItemID));
                _uniqueAssertion.AssertUnique(creation.ItemID, _grantPolicyRepository.Contains(creation.ItemID));
                _collectionAssertion.AssertHasElements(creation.LootTableEntries);
                
                CreateLootTable(creation.LootTableEntries, creation.ItemID);
                CreateGrantPolicy(creation.GrantPolicyEntry, creation.ItemID);
                
                HarvestNodeLootCreationResponse response = new() { ItemID = creation.ItemID, LootTableEntries = creation.LootTableEntries };
                responses[i] = response;
            }
            
            _responseDispatcher.Dispatch(responses);
        }

        private void CreateGrantPolicy(GrantPolicyEntry grantPolicyEntry, ItemID itemID)
        {
            if (grantPolicyEntry.SkipWeight == 0)
            {
                _grantPolicyRepository.Add(itemID, new GrantPolicy());
                return;
            }
            
            ILootRoll lootRoll = new DefaultLootRoll();
            WeightedPolicy weightedPolicy = _policyFactory.Create(grantPolicyEntry, lootRoll);
            _grantPolicyRepository.Add(itemID, weightedPolicy);
        }

        private void CreateLootTable(LootTableEntry[] lootTableEntries, ItemID itemID)
        {
            if (lootTableEntries.Length == 1)
            {
                _lootTableRepository.Add(itemID, new GrantTable { ItemID = itemID });
                return;
            }

            WeightedEntry[] entries = new WeightedEntry[lootTableEntries.Length];
            for (int i = 0; i < lootTableEntries.Length; i++)
            {
                LootTableEntry entry = lootTableEntries[i];
                
                WeightedEntry weightedEntry = new() { ItemID = entry.ItemID, Weight = entry.Weight };
                entries[i] = weightedEntry;
            }

            ILootRoll lootRoll = new DefaultLootRoll();
            ILootTable lootTable = _lootTableFactory.Create(entries, lootRoll);
            _lootTableRepository.Add(itemID, lootTable);
        }
    }
}
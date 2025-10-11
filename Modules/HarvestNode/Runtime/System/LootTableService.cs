using IdelPog.Core.Contracts;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.HarvestNode.Runtime.Factory.Interface;
using IdelPog.HarvestNode.Runtime.System.Interface;
using IdelPog.Loot.Random;
using IdelPog.Loot.Table;

namespace IdelPog.HarvestNode.Runtime.System
{
    public sealed class LootTableService<TID> : ILootTableService<TID>
    {
        private readonly IAssetRepository<TID, ILootTable> _lootTableRepository;
        private readonly IWeightedLootTableFactory _lootTableFactory;
        private readonly IUniqueAssertion _uniqueAssertion;

        public LootTableService(IAssetRepository<TID, ILootTable> lootTableRepository, IWeightedLootTableFactory lootTableFactory, IUniqueAssertion uniqueAssertion)
        {
            _lootTableRepository = lootTableRepository;
            _lootTableFactory = lootTableFactory;
            _uniqueAssertion = uniqueAssertion;
        }

        public void CreateLootTable(LootTableEntry[] lootTableEntries, TID id)
        {
            _uniqueAssertion.AssertUnique(id, _lootTableRepository.Contains(id));
            
            if (lootTableEntries.Length == 1)
            {
                _lootTableRepository.Add(id, new GrantTable { ItemID = lootTableEntries[0].ItemID });
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
            _lootTableRepository.Add(id, lootTable);
        }
    }
}
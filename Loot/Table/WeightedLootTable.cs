using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Loot.Assertion.Interface;
using IdelPog.Loot.Random;

namespace IdelPog.Loot.Table
{
    public sealed class WeightedLootTable : ILootTable
    {
        private readonly WeightedEntry[] _entries;
        private readonly int _maxWeight;
        private readonly ILootRoll _lootRoll;

        public WeightedLootTable(WeightedEntry[] entries, ILootRoll lootRoll, ICollectionAssertion collectionAssertion, IWeightAssertion weightAssertion)
        {
            _entries = entries;
            _lootRoll = lootRoll;

            collectionAssertion.AssertHasElements(_entries);

            _maxWeight = 0;
            foreach (WeightedEntry weightedEntry in entries)
            {
                weightAssertion.AssertWeightIsPositive(weightedEntry.Weight);
                _maxWeight += weightedEntry.Weight;
            }
        }

        public ItemID Roll()
        {
            int roll = _lootRoll.ExclusiveNextInt(0, _maxWeight);
            foreach (WeightedEntry weightedEntry in _entries)
            {
                if (roll < weightedEntry.Weight)
                {
                    return weightedEntry.ItemID;
                }
                
                roll -= weightedEntry.Weight;
            }
            
            throw new InvalidOperationException("Roll out of range");
        }
    }
}
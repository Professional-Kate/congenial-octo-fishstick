using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Loot.Assertion.Interface;
using IdelPog.Loot.Random;

namespace IdelPog.Loot.Contracts
{
    public sealed class WeightedLootTable : ILootTable
    {
        private readonly WeightedEntry[] _entries;
        private readonly uint _maxWeight;
        private readonly ILootRoll _lootRoll;

        public WeightedLootTable(WeightedEntry[] entries, ILootRoll lootRoll, ICollectionAssertion collectionAssertion, IWeightAssertion weightAssertion)
        {
            _entries = entries;
            _lootRoll = lootRoll;

            collectionAssertion.AssertHasElements(_entries);

            _maxWeight = 0;
            foreach (WeightedEntry weightedEntry in entries)
            {
                _maxWeight += weightedEntry.Weight;
            }

            weightAssertion.AssertWeightIsNotZero(_maxWeight);
        }

        public ItemID Roll()
        {
            uint roll = _lootRoll.ExclusiveNextInt(0, _maxWeight);
            foreach (WeightedEntry weightedEntry in _entries)
            {
                if (roll < weightedEntry.Weight)
                {
                    return weightedEntry.ItemID;
                }
                
                roll -= weightedEntry.Weight;
            }
            
            throw new InvalidOperationException("Number out of range");
        }
    }
}
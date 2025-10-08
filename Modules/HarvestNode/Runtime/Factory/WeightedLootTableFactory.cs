using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.HarvestNode.Runtime.Factory.Interfaces;
using IdelPog.Loot.Assertion.Interface;
using IdelPog.Loot.Random;
using IdelPog.Loot.Table;

namespace IdelPog.HarvestNode.Runtime.Factory
{
    public sealed class WeightedLootTableFactory : IWeightedLootTableFactory
    {
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IWeightAssertion _weightAssertion;

        public WeightedLootTableFactory(ICollectionAssertion collectionAssertion, IWeightAssertion weightAssertion)
        {
            _collectionAssertion = collectionAssertion;
            _weightAssertion = weightAssertion;
        }

        public WeightedLootTable Create(WeightedEntry[] entries, ILootRoll lootRoll)
        {
            return new WeightedLootTable(entries, lootRoll, _collectionAssertion, _weightAssertion);
        }
    }
}
using IdelPog.Loot.Random;
using IdelPog.Loot.Table;

namespace IdelPog.HarvestNode.Runtime.Factory.Interface
{
    public interface IWeightedLootTableFactory
    {
        public WeightedLootTable Create(WeightedEntry[] entries, ILootRoll lootRoll);
    }
}
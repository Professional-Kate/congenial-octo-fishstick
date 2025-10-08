using IdelPog.Loot.Random;
using IdelPog.Loot.Table;

namespace IdelPog.HarvestNode.Runtime.Factory.Interfaces
{
    public interface IWeightedLootTableFactory
    {
        public WeightedLootTable Create(WeightedEntry[] entries, ILootRoll lootRoll);
    }
}
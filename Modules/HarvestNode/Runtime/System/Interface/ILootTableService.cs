using IdelPog.Core.Contracts;

namespace IdelPog.HarvestNode.Runtime.System.Interface
{
    public interface ILootTableService<in TID>
    {
        public void CreateLootTable(LootTableEntry[] lootTableEntries, TID id);
    }
}
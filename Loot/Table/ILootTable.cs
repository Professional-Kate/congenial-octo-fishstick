using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Loot.Table
{
    public interface ILootTable
    {
        public ItemID Roll();
    }
}
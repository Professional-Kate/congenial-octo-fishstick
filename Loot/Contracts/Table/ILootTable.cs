using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Loot.Contracts.Table
{
    public interface ILootTable
    {
        public ItemID Roll();
    }
}
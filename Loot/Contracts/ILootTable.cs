using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Loot.Contracts
{
    public interface ILootTable
    {
        public ItemID Roll();
    }
}
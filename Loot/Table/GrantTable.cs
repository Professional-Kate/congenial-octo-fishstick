using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Loot.Table
{
    public readonly record struct GrantTable : ILootTable
    {
        public required ItemID ItemID { get; init; }

        public ItemID Roll()
        {
            return ItemID;
        }
    }
}
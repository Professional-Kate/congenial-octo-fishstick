using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Loot.Contracts.Table
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
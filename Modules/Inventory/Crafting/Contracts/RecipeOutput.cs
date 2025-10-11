using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Inventory.Crafting.Contracts
{
    public readonly record struct RecipeOutput
    {
        public required ItemID ItemID { get; init; }
        public required byte Amount { get; init; }
    }
}
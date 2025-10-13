using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Inventory.Contracts
{
    public readonly record struct RecipeInput
    {
        public required ItemID ItemID { get; init; }
        public required byte Amount { get; init; }
    }
}
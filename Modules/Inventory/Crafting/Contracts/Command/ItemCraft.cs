namespace IdelPog.Inventory.Crafting.Contracts.Command
{
    public readonly record struct ItemCraft
    {
        public required RecipeID RecipeID { get; init; }
        public required byte Amount { get; init; }
    }
}
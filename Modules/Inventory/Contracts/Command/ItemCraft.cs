namespace IdelPog.Inventory.Contracts.Command
{
    public readonly record struct ItemCraft
    {
        public required RecipeID RecipeID { get; init; }
        public required uint Amount { get; init; }
    }
}
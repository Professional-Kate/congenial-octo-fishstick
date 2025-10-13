namespace IdelPog.Inventory.Contracts.Command
{
    public readonly record struct ItemCraft
    {
        public required RecipeID RecipeID { get; init; }
        public required byte Amount { get; init; }
    }
}
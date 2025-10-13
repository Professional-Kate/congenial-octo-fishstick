namespace IdelPog.Inventory.Contracts.Response
{
    public readonly record struct ItemCraftResponse
    {
        public required RecipeID RecipeID { get; init; }
        public required uint Amount { get; init; }
    }
}
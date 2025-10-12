namespace IdelPog.Inventory.Crafting.Contracts.Response
{
    public readonly record struct ItemCraftResponse
    {
        public required RecipeID RecipeID { get; init; }
    }
}
namespace IdelPog.Inventory.Crafting.Contracts.Command
{
    public readonly record struct RecipeCreation
    {
        public required RecipeID RecipeID { get; init; }
        public required RecipeInput[] RecipeInputs { get; init; }
        public required RecipeOutput[] RecipeOutputs { get; init; }
    }
}
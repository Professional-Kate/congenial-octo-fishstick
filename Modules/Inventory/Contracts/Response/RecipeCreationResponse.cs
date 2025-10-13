namespace IdelPog.Inventory.Contracts.Response
{
    public readonly record struct RecipeCreationResponse
    {
        public required RecipeID RecipeID { get; init; }
        public required RecipeInput[] RecipeInputs { get; init; }
        public required RecipeOutput[] RecipeOutputs { get; init; }
    }
}
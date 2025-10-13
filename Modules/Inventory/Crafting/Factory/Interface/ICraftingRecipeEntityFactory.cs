using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Crafting.ECS;

namespace IdelPog.Inventory.Crafting.Factory.Interface
{
    public interface ICraftingRecipeEntityFactory
    {
        public CraftingRecipeEntity Create(RecipeInput[] recipeInputs, RecipeOutput[] recipeOutputs);
    }
}
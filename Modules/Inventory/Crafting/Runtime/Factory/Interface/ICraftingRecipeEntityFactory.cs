using IdelPog.Inventory.Crafting.Contracts;
using IdelPog.Inventory.Crafting.Runtime.ECS;

namespace IdelPog.Inventory.Crafting.Runtime.Factory.Interface
{
    public interface ICraftingRecipeEntityFactory
    {
        public CraftingRecipeEntity Create(RecipeInput[] recipeInputs, RecipeOutput[] recipeOutputs);
    }
}
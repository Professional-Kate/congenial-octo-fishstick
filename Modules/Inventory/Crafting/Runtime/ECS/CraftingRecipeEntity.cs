using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.ECS.Component;
using IdelPog.ECS.Entity;
using IdelPog.Inventory.Crafting.Runtime.ECS.Component;

namespace IdelPog.Inventory.Crafting.Runtime.ECS
{
    public sealed record CraftingRecipeEntity : Entity
    {
        private readonly ComponentStore<RecipeInputComponent> _ingredientStore;
        private readonly ComponentStore<RecipeOutputComponent> _outputStore;

        public CraftingRecipeEntity(RecipeInputComponent[] inputs, RecipeOutputComponent[] outputs, IHandler handler) 
            : base(new ComponentStore<RecipeInputComponent>(inputs, handler),  new ComponentStore<RecipeOutputComponent>(outputs, handler))
        {
            _ingredientStore = GetComponent<ComponentStore<RecipeInputComponent>>();
            _outputStore = GetComponent<ComponentStore<RecipeOutputComponent>>();
        }

        public bool ContainsRecipe(Predicate<RecipeOutputComponent> predicate)
        {
            return _outputStore.ContainsComponent(predicate);
        }
    }
}
using IdelPog.ECS.Component;
using IdelPog.ECS.Entity;
using IdelPog.Inventory.Crafting.ECS.Component;

namespace IdelPog.Inventory.Crafting.ECS
{
    public sealed record CraftingRecipeEntity : Entity
    {
        private readonly ComponentStore<RecipeInputComponent> _ingredientStore;
        private readonly ComponentStore<RecipeOutputComponent> _outputStore;

        public CraftingRecipeEntity(RecipeInputComponent[] inputs, RecipeOutputComponent[] outputs) 
            : base(new ComponentStore<RecipeOutputComponent>(outputs), new ComponentStore<RecipeInputComponent>(inputs))
        {
            _ingredientStore = GetComponent<ComponentStore<RecipeInputComponent>>();
            _outputStore = GetComponent<ComponentStore<RecipeOutputComponent>>();
        }

        public bool ContainsRecipe(Predicate<RecipeOutputComponent> predicate)
        {
            return _outputStore.ContainsComponent(predicate);
        }
        
        public RecipeInputComponent[] GetRecipe()
        { 
            return _ingredientStore.GetAllComponents();
        }

        public RecipeOutputComponent[] GetOutput()
        { 
            return _outputStore.GetAllComponents();
        }
    }
}
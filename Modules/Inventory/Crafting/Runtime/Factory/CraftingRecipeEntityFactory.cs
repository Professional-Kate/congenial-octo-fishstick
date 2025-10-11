using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Inventory.Crafting.Contracts;
using IdelPog.Inventory.Crafting.Runtime.ECS;
using IdelPog.Inventory.Crafting.Runtime.ECS.Component;
using IdelPog.Inventory.Crafting.Runtime.Factory.Interface;

namespace IdelPog.Inventory.Crafting.Runtime.Factory
{
    public sealed class CraftingRecipeEntityFactory : ICraftingRecipeEntityFactory
    {
        private readonly IHandler _handler;
        private readonly ICollectionAssertion _collectionAssertion;

        public CraftingRecipeEntityFactory(IHandler handler, ICollectionAssertion collectionAssertion)
        {
            _handler = handler;
            _collectionAssertion = collectionAssertion;
        }

        public CraftingRecipeEntity Create(RecipeInput[] recipeInputs, RecipeOutput[] recipeOutputs)
        {
            _collectionAssertion.AssertHasElements(recipeInputs);
            _collectionAssertion.AssertHasElements(recipeOutputs);
            
            return new CraftingRecipeEntity(CreateInputComponents(recipeInputs), CreateOutputComponents(recipeOutputs), _handler);
        }

        private static RecipeInputComponent[] CreateInputComponents(RecipeInput[] recipeInputs)
        {
            RecipeInputComponent[] components = new RecipeInputComponent[recipeInputs.Length];
            for (int i = 0; i < recipeInputs.Length; i++)
            {
                RecipeInput recipeInput = recipeInputs[i];
                
                RecipeInputComponent component = new() { ItemID = recipeInput.ItemID, RequiredAmount = recipeInput.Amount };
                
                components[i] = component;
            }
            
            return components;
        }

        private static RecipeOutputComponent[] CreateOutputComponents(RecipeOutput[] recipeOutputs)
        {
            RecipeOutputComponent[] components = new RecipeOutputComponent[recipeOutputs.Length];
            for (var i = 0; i < recipeOutputs.Length; i++)
            {
                RecipeOutput recipeOutput = recipeOutputs[i];

                RecipeOutputComponent component = new() { ItemID = recipeOutput.ItemID, OutputAmount = recipeOutput.Amount };
                
                components[i] = component;
            }
            
            return components;
        }
    }
}
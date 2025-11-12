using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Crafting.ECS;
using IdelPog.Inventory.Crafting.ECS.Component;
using IdelPog.Inventory.Crafting.Factory.Interface;

namespace IdelPog.Inventory.Crafting.Factory
{
    public sealed class CraftingRecipeEntityFactory : ICraftingRecipeEntityFactory
    {
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IAmountAssertion _amountAssertion;
        private readonly IRepositoryAsserter _repositoryAsserter;

        public CraftingRecipeEntityFactory(ICollectionAssertion collectionAssertion, IRepositoryAsserter repositoryAsserter)
        {
            _collectionAssertion = collectionAssertion;
            _repositoryAsserter = repositoryAsserter;
            _amountAssertion = new AmountAssertion();
        }

        public CraftingRecipeEntity Create(RecipeInput[] recipeInputs, RecipeOutput[] recipeOutputs)
        {
            _collectionAssertion.AssertHasElements(recipeInputs);
            _collectionAssertion.AssertHasElements(recipeOutputs);
            
            return new CraftingRecipeEntity(_repositoryAsserter, CreateInputComponents(recipeInputs), CreateOutputComponents(recipeOutputs));
        }

        private RecipeInputComponent[] CreateInputComponents(RecipeInput[] recipeInputs)
        {
            RecipeInputComponent[] components = new RecipeInputComponent[recipeInputs.Length];
            for (int i = 0; i < recipeInputs.Length; i++)
            {
                RecipeInput recipeInput = recipeInputs[i];
                _amountAssertion.AssertAmountNotZero(recipeInput.Amount);
                
                RecipeInputComponent component = new() { ItemID = recipeInput.ItemID, RequiredAmount = recipeInput.Amount };
                
                components[i] = component;
            }
            
            return components;
        }

        private RecipeOutputComponent[] CreateOutputComponents(RecipeOutput[] recipeOutputs)
        {
            RecipeOutputComponent[] components = new RecipeOutputComponent[recipeOutputs.Length];
            for (var i = 0; i < recipeOutputs.Length; i++)
            {
                RecipeOutput recipeOutput = recipeOutputs[i];
                _amountAssertion.AssertAmountNotZero(recipeOutput.Amount);

                RecipeOutputComponent component = new() { ItemID = recipeOutput.ItemID, OutputAmount = recipeOutput.Amount };
                
                components[i] = component;
            }
            
            return components;
        }
    }
}
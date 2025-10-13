using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Crafting.ECS;
using IdelPog.Inventory.Crafting.Factory.Interface;

namespace IdelPog.Inventory.Crafting.Mediator
{
    public sealed class RecipeCreationMediator : IBatchMediator<RecipeCreation>
    {
        private readonly IAssetRepository<RecipeID, CraftingRecipeEntity> _recipeEntityRepository;
        private readonly ICraftingRecipeEntityFactory _entityFactory;
        private readonly IDispatchMany<RecipeCreationResponse> _responseDispatcher;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IUniqueAssertion _uniqueAssertion;

        public RecipeCreationMediator(IAssetRepository<RecipeID, CraftingRecipeEntity> recipeEntityRepository, ICraftingRecipeEntityFactory entityFactory, IDispatchMany<RecipeCreationResponse> responseDispatcher, ICollectionAssertion collectionAssertion, IUniqueAssertion uniqueAssertion)
        {
            _recipeEntityRepository = recipeEntityRepository;
            _entityFactory = entityFactory;
            _responseDispatcher = responseDispatcher;
            _collectionAssertion = collectionAssertion;
            _uniqueAssertion = uniqueAssertion;
        }

        public void HandleMessages(IReadOnlyList<RecipeCreation> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            RecipeCreationResponse[] responses = new RecipeCreationResponse[messages.Count];
            for (int i = 0; i < messages.Count; i++)
            {
                RecipeCreation recipeCreation = messages[i];
                AssertRecipeCreation(recipeCreation);
                
                _recipeEntityRepository.Add(recipeCreation.RecipeID, _entityFactory.Create(recipeCreation.RecipeInputs, recipeCreation.RecipeOutputs));

                RecipeCreationResponse response = new() { RecipeID = recipeCreation.RecipeID, RecipeInputs = recipeCreation.RecipeInputs, RecipeOutputs = recipeCreation.RecipeOutputs };
                responses[i] = response;
            }
            
            _responseDispatcher.Dispatch(responses);
        }

        private void AssertRecipeCreation(RecipeCreation recipeCreation)
        {
            _collectionAssertion.AssertHasElements(recipeCreation.RecipeInputs);
            _collectionAssertion.AssertHasElements(recipeCreation.RecipeOutputs);
            _uniqueAssertion.AssertUnique(recipeCreation.RecipeID, _recipeEntityRepository.Contains(recipeCreation.RecipeID));
        }
    }
}
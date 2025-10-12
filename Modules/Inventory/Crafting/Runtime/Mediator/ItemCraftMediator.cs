using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Inventory.Assertion.Interface;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Crafting.Contracts;
using IdelPog.Inventory.Crafting.Contracts.Command;
using IdelPog.Inventory.Crafting.Contracts.Response;
using IdelPog.Inventory.Crafting.Runtime.ECS;
using IdelPog.Inventory.Crafting.Runtime.ECS.Component;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory.Crafting.Runtime.Mediator
{
    public sealed class ItemCraftMediator : IBatchMediator<ItemCraft>
    {
        private readonly IInventory _inventory;
        private readonly IAssetRepository<RecipeID, CraftingRecipeEntity> _recipeEntityRepository;
        private readonly IInventoryUpdateService _inventoryUpdateService;
        private readonly IDispatchMany<ItemCraftResponse> _responseDispatcher;
        private readonly IFoundAssertion _foundAssertion;
        private readonly IAmountAssertion _amountAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public ItemCraftMediator(IInventory inventory, IAssetRepository<RecipeID, CraftingRecipeEntity> recipeEntityRepository, IInventoryUpdateService inventoryUpdateService, IDispatchMany<ItemCraftResponse> responseDispatcher, IFoundAssertion foundAssertion, IAmountAssertion amountAssertion, ICollectionAssertion collectionAssertion)
        {
            _inventory = inventory;
            _recipeEntityRepository = recipeEntityRepository;
            _inventoryUpdateService = inventoryUpdateService;
            _responseDispatcher = responseDispatcher;
            _foundAssertion = foundAssertion;
            _amountAssertion = amountAssertion;
            _collectionAssertion = collectionAssertion;
        }

        public void HandleMessages(IReadOnlyList<ItemCraft> messages)
        {
            _collectionAssertion.AssertHasElements(messages);
            
            ItemCraftResponse[] responses = new ItemCraftResponse[messages.Count];
            List<InventoryUpdate> updates = new(messages.Count);
            
            for (var i = 0; i < messages.Count; i++)
            {
                ItemCraft itemCraft = messages[i];
                _foundAssertion.AssertFound(itemCraft.RecipeID, _recipeEntityRepository.Contains(itemCraft.RecipeID));
                
                CraftingRecipeEntity entity = _recipeEntityRepository.Get(itemCraft.RecipeID);
                
                updates.AddRange(CreateRemoveInventoryUpdates(entity.GetRecipe()));
                updates.AddRange(CreateAddInventoryUpdates(entity.GetOutput()));

                ItemCraftResponse response = new(){ RecipeID = itemCraft.RecipeID };
                responses[i] = response;
            }
            
            _inventoryUpdateService.ApplyUpdates(updates);
            _responseDispatcher.Dispatch(responses);
        }

        private InventoryUpdate[] CreateRemoveInventoryUpdates(RecipeInputComponent[] components)
        {
            InventoryUpdate[] updates = new InventoryUpdate[components.Length];
            for (int i = 0; i < components.Length; i++)
            {
                RecipeInputComponent recipeInputComponent = components[i];
                _foundAssertion.AssertFound(recipeInputComponent.ItemID, _inventory.Contains(recipeInputComponent.ItemID));

                Item item = _inventory.GetItem(recipeInputComponent.ItemID);
                _amountAssertion.AssertEnoughAmount(recipeInputComponent.RequiredAmount, item.Amount, item.ItemID);

                updates[i] = _inventoryUpdateService.CreateRemoveUpdate(recipeInputComponent.ItemID, recipeInputComponent.RequiredAmount);
            }

            return updates;
        }

        private InventoryUpdate[] CreateAddInventoryUpdates(RecipeOutputComponent[] components)
        {
            InventoryUpdate[] updates = new InventoryUpdate[components.Length];
            for (var i = 0; i < components.Length; i++)
            {
                RecipeOutputComponent recipeOutputComponent = components[i];

                InventoryUpdate inventoryUpdate = _inventoryUpdateService.CreateAddUpdate(recipeOutputComponent.ItemID, recipeOutputComponent.OutputAmount);
                updates[i] = inventoryUpdate;
            }

            return updates;
        }
    }
}
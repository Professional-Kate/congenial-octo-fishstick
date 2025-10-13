using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Inventory.Assertion.Interface;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Crafting.ECS;
using IdelPog.Inventory.Crafting.ECS.Component;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory.Crafting.Mediator
{
    public sealed class ItemCraftMediator : IBatchMediator<ItemCraft>
    {
        private readonly IInventory _inventory;
        private readonly IAssetRepository<RecipeID, CraftingRecipeEntity> _recipeEntityRepository;
        private readonly IInventoryUpdateService _inventoryUpdateService;
        private readonly IInventoryUpdateFactory _inventoryUpdateFactory;
        private readonly IDispatchMany<ItemCraftResponse> _itemCraftDispatcher;
        private readonly IDispatchMany<InventoryUpdateResponse> _inventoryUpdateDispatcher;
        private readonly IFoundAssertion _foundAssertion;
        private readonly IAmountAssertion _amountAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public ItemCraftMediator(IInventory inventory, IAssetRepository<RecipeID, CraftingRecipeEntity> recipeEntityRepository,
            IInventoryUpdateService inventoryUpdateService, IInventoryUpdateFactory inventoryUpdateFactory, IDispatchMany<ItemCraftResponse> itemCraftDispatcher,
            IDispatchMany<InventoryUpdateResponse> inventoryUpdateDispatcher, IFoundAssertion foundAssertion, IAmountAssertion amountAssertion,
            ICollectionAssertion collectionAssertion)
        {
            _inventory = inventory;
            _recipeEntityRepository = recipeEntityRepository;
            _inventoryUpdateService = inventoryUpdateService;
            _inventoryUpdateFactory = inventoryUpdateFactory;
            _itemCraftDispatcher = itemCraftDispatcher;
            _inventoryUpdateDispatcher = inventoryUpdateDispatcher;
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
            
            IReadOnlyList<InventoryUpdateResponse> inventoryUpdateResponses = _inventoryUpdateService.ApplyUpdates(updates);
             
            _inventoryUpdateDispatcher.Dispatch(inventoryUpdateResponses);
            _itemCraftDispatcher.Dispatch(responses);
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

                updates[i] = _inventoryUpdateFactory.Create(recipeInputComponent.ItemID, recipeInputComponent.RequiredAmount, ActionType.REMOVE);
            }

            return updates;
        }

        private InventoryUpdate[] CreateAddInventoryUpdates(RecipeOutputComponent[] components)
        {
            InventoryUpdate[] updates = new InventoryUpdate[components.Length];
            for (var i = 0; i < components.Length; i++)
            {
                RecipeOutputComponent recipeOutputComponent = components[i];

                InventoryUpdate inventoryUpdate = _inventoryUpdateFactory.Create(recipeOutputComponent.ItemID, recipeOutputComponent.OutputAmount, ActionType.ADD);
                updates[i] = inventoryUpdate;
            }

            return updates;
        }
    }
}
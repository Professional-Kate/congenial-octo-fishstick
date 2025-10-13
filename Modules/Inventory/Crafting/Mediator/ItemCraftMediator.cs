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
        private readonly IInventoryUpdateSummarizer _inventoryUpdateSummarizer;
        private readonly IDispatchMany<ItemCraftResponse> _itemCraftDispatcher;
        private readonly IDispatchMany<InventoryUpdateResponse> _inventoryUpdateDispatcher;
        private readonly IFoundAssertion _foundAssertion;
        private readonly IAmountAssertion _amountAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public ItemCraftMediator(IInventory inventory, IAssetRepository<RecipeID, CraftingRecipeEntity> recipeEntityRepository,
            IInventoryUpdateService inventoryUpdateService, IInventoryUpdateFactory inventoryUpdateFactory, IInventoryUpdateSummarizer inventoryUpdateSummarizer, IDispatchMany<ItemCraftResponse> itemCraftDispatcher,
            IDispatchMany<InventoryUpdateResponse> inventoryUpdateDispatcher, IFoundAssertion foundAssertion, IAmountAssertion amountAssertion,
            ICollectionAssertion collectionAssertion)
        {
            _inventory = inventory;
            _recipeEntityRepository = recipeEntityRepository;
            _inventoryUpdateService = inventoryUpdateService;
            _inventoryUpdateFactory = inventoryUpdateFactory;
            _inventoryUpdateSummarizer = inventoryUpdateSummarizer;
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
                
                updates.AddRange(CreateRemoveInventoryUpdates(entity.GetRecipe(), itemCraft.Amount));
                updates.AddRange(CreateAddInventoryUpdates(entity.GetOutput(), itemCraft.Amount));

                ItemCraftResponse response = new(){ RecipeID = itemCraft.RecipeID, Amount = itemCraft.Amount };
                responses[i] = response;
            }

            IReadOnlyList<InventoryUpdate> summarizedUpdates = _inventoryUpdateSummarizer.GetSummary(updates);
            IReadOnlyList<InventoryUpdateResponse> inventoryUpdateResponses = _inventoryUpdateService.ApplyUpdates(summarizedUpdates);
             
            _inventoryUpdateDispatcher.Dispatch(inventoryUpdateResponses);
            _itemCraftDispatcher.Dispatch(responses);
        }

        private InventoryUpdate[] CreateRemoveInventoryUpdates(RecipeInputComponent[] components, uint iterations)
        {
            List<InventoryUpdate> updates = new(components.Length);
            foreach (RecipeInputComponent recipeInputComponent in components)
            {
                _foundAssertion.AssertFound(recipeInputComponent.ItemID, _inventory.Contains(recipeInputComponent.ItemID));

                Item item = _inventory.GetItem(recipeInputComponent.ItemID);
                _amountAssertion.AssertEnoughAmount(recipeInputComponent.RequiredAmount, item.Amount, item.ItemID);
                
                InventoryUpdate[] inventoryUpdates = _inventoryUpdateFactory.CreateMultiple(recipeInputComponent.ItemID, recipeInputComponent.RequiredAmount, ActionType.REMOVE, iterations);
                updates.AddRange(inventoryUpdates);
            }

            return updates.ToArray();
        }

        private InventoryUpdate[] CreateAddInventoryUpdates(RecipeOutputComponent[] components, uint iterations)
        {
            List<InventoryUpdate> updates = new(components.Length);
            foreach (RecipeOutputComponent recipeOutputComponent in components)
            {
                InventoryUpdate[] inventoryUpdates = _inventoryUpdateFactory.CreateMultiple(recipeOutputComponent.ItemID, recipeOutputComponent.OutputAmount, ActionType.ADD, iterations);
                updates.AddRange(inventoryUpdates);
            }

            return updates.ToArray();
        }
    }
}
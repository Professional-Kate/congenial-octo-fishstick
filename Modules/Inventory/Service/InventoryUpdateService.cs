using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Inventory.Assertion.Interface;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory.Service
{
    public sealed class InventoryUpdateService : IInventoryUpdateService
    {
        private readonly IInventory _inventory;
        private readonly IItemInfoFactory _itemInfoFactory;
        private readonly IItemCreationService _itemCreationService;
        private readonly ICollectionAssertion _collectionAssertion;
        private readonly IItemFoundAssertion _itemFoundAssertion;

        public InventoryUpdateService(IInventory inventory, IItemInfoFactory itemInfoFactory, ICollectionAssertion collectionAssertion, IItemFoundAssertion itemFoundAssertion, IItemCreationService itemCreationService)
        {
            _inventory = inventory;
            _itemInfoFactory = itemInfoFactory;
            _collectionAssertion = collectionAssertion;
            _itemFoundAssertion = itemFoundAssertion;
            _itemCreationService = itemCreationService;
        }

        public IReadOnlyList<InventoryUpdateResponse> ApplyUpdates(IReadOnlyList<InventoryUpdate> inventoryUpdates)
        {
            _collectionAssertion.AssertHasElements(inventoryUpdates);
            
            InventoryUpdateResponse[] responses = new InventoryUpdateResponse[inventoryUpdates.Count];
            for (int i = 0; i < inventoryUpdates.Count; i++)
            {
                InventoryUpdate inventoryUpdate = inventoryUpdates[i];
                bool contains = _inventory.Contains(inventoryUpdate.ItemID);

                if (contains == false)
                {
                    responses[i] = HandleMissingItem(inventoryUpdate);
                    continue;
                }

                switch (inventoryUpdate.ActionType)
                {
                    case ActionType.ADD:
                        responses[i] = HandleAdd(inventoryUpdate);
                        break;
                    case ActionType.REMOVE:
                        responses[i] = HandleRemove(inventoryUpdate);
                        break;
                }
            }
            
            return responses;
        }

        private InventoryUpdateResponse HandleMissingItem(InventoryUpdate update)
        {
            _itemFoundAssertion.AssertItemFound(false, update.ActionType, update.ItemID);

            Item item = _itemCreationService.Create(update.ItemID, update.Amount);
            _inventory.AddItem(item);

            return CreateInventoryUpdateResponse(CreateItemInfo(item), MutateType.CREATED);
        }

        private InventoryUpdateResponse HandleAdd(InventoryUpdate update)
        {
            _inventory.AddAmount(update.ItemID, update.Amount);
            Item item = _inventory.GetItem(update.ItemID);

            return CreateInventoryUpdateResponse(CreateItemInfo(item), MutateType.CHANGED);
        }

        private InventoryUpdateResponse HandleRemove(InventoryUpdate update)
        {
            Item item = _inventory.GetItem(update.ItemID);
            MutateType mutateType = _inventory.RemoveAmount(update.ItemID,  update.Amount);

            if (mutateType == MutateType.DELETED)
            {
                item.Amount = 0;
                return CreateInventoryUpdateResponse(CreateItemInfo(item), mutateType);
            }
                        
            return CreateInventoryUpdateResponse(CreateItemInfo(_inventory.GetItem(update.ItemID)), mutateType);
        }

        private static InventoryUpdateResponse CreateInventoryUpdateResponse(ItemInfo itemInfo, MutateType mutateType)
        {
            return new InventoryUpdateResponse
            {
                ItemInfo = itemInfo,
                MutateType = mutateType
            };
        }

        private ItemInfo CreateItemInfo(Item item)
        {
            return _itemInfoFactory.Create(item.ItemID, item.BaseSellPrice, item.Amount, item.Information);
        }
    }
}
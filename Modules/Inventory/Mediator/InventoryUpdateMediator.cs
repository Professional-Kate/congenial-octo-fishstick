using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Information;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory.Mediator
{
    public sealed class InventoryUpdateMediator : IBatchMediator<InventoryUpdate>
    {
        private readonly IInventory _inventory;
        private readonly IItemFactory _itemFactory;
        private readonly IInventoryUpdateSummarizer _updateSummarizer;
        private readonly IInventoryUpdateResponseFactory _inventoryUpdateResponseFactory;
        private readonly IItemInfoFactory _itemInfoFactory;
        private readonly IDispatchMany<InventoryUpdateResponse> _inventoryUpdateDispatcher;
        private readonly IMapper<ItemID> _itemMapper;
        private readonly ICollectionAssertion _collectionAssertion;

        public InventoryUpdateMediator(IInventory inventory, IItemFactory itemFactory, IInventoryUpdateSummarizer updateSummarizer,
            IInventoryUpdateResponseFactory inventoryUpdateResponseFactory, IItemInfoFactory itemInfoFactory, IMapper<ItemID> itemMapper, 
            IDispatchMany<InventoryUpdateResponse> inventoryUpdateDispatcher, ICollectionAssertion collectionAssertion)
        {
            _inventory = inventory;
            _itemFactory = itemFactory;
            _updateSummarizer = updateSummarizer;
            _inventoryUpdateResponseFactory = inventoryUpdateResponseFactory;
            _itemInfoFactory = itemInfoFactory;
            _inventoryUpdateDispatcher = inventoryUpdateDispatcher;
            _collectionAssertion = collectionAssertion;
            _itemMapper = itemMapper;
        }

        public void HandleMessages(IReadOnlyList<InventoryUpdate> updates)
        {
            _collectionAssertion.AssertHasElements(updates);
            IReadOnlyList<InventoryUpdate> summaryUpdates = _updateSummarizer.GetSummary(updates);
            _collectionAssertion.AssertHasElements(summaryUpdates);
            
            InventoryUpdateResponse[] responses = new InventoryUpdateResponse[summaryUpdates.Count];
            for (int i = 0; i < summaryUpdates.Count; i++)
            {
                InventoryUpdate update = summaryUpdates[i];
                MutateType mutateType = MutateType.CHANGED;

                switch (update.ActionType)
                {
                    case ActionType.ADD:
                        mutateType = CreateOrIncreaseAmount(update.ItemID, update.Amount);
                        break;
                    case ActionType.REMOVE:
                        mutateType = _inventory.RemoveAmount(update.ItemID, update.Amount);
                        break;
                }

                ItemInfo itemInfo;
                if (mutateType == MutateType.DELETED)
                {
                    itemInfo = _itemInfoFactory.Create(update.ItemID, 0, 0, _itemMapper.GetInformation(update.ItemID));
                }
                else
                {
                    Item item = _inventory.GetItem(update.ItemID);
                    itemInfo = _itemInfoFactory.Create(update.ItemID, item.BaseSellPrice, item.Amount, _itemMapper.GetInformation(update.ItemID));
                }

                responses[i] = _inventoryUpdateResponseFactory.Create(itemInfo, mutateType);
            }

            _inventoryUpdateDispatcher.Dispatch(responses);
        }

        /// <summary>
        /// Adds an amount to an <see cref="Item"/>, the <see cref="Item"/> will be found by using its connected <see cref="ItemID"/>
        /// </summary>
        /// <param name="itemID">The <see cref="Item"/> you want to add will have this <see cref="ItemID"/></param>
        /// <param name="amount">The amount you want to add</param>
        /// <remarks>If the <see cref="Item"/> with the passed <see cref="ItemID"/> is not found, it will be created</remarks>
        private MutateType CreateOrIncreaseAmount(ItemID itemID, uint amount)
        {
            if (_inventory.Contains(itemID))
            {
                _inventory.AddAmount(itemID, amount);
                return MutateType.CHANGED;
            }

            // if an Item doesn't exist then we create one
            Item createdItem = _itemFactory.CreateItem(itemID, amount);
            _inventory.AddItem(createdItem);
            return MutateType.CREATED;
        }
    }
}
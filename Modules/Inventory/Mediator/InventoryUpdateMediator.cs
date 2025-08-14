using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Service;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory.Mediator
{
    public class InventoryUpdateMediator : IBatchMediator<InventoryUpdate>
    {
        private readonly IInventory _inventory;
        private readonly IItemFactory _itemFactory;
        private readonly IInventoryUpdateResponseFactory _inventoryUpdateResponseFactory;
        private readonly IDispatchMany<InventoryUpdateResponse> _inventoryUpdateDispatcher;

        public InventoryUpdateMediator(IInventory inventory, IItemFactory itemFactory, IInventoryUpdateResponseFactory inventoryUpdateResponseFactory,
            IDispatchMany<InventoryUpdateResponse> inventoryUpdateDispatcher)
        {
            _inventory = inventory;
            _itemFactory = itemFactory;
            _inventoryUpdateResponseFactory = inventoryUpdateResponseFactory;
            _inventoryUpdateDispatcher = inventoryUpdateDispatcher;
        }

        public void HandleMessages(IReadOnlyList<InventoryUpdate> updates)
        {
            List<InventoryUpdateResponse> responses = new(updates.Count);

            foreach (InventoryUpdate update in updates)
            {
                MutateType mutateType;

                switch (update.Action)
                {
                    case ActionType.ADD:
                        mutateType = CreateOrIncreaseAmount(update.ItemID, update.Amount);
                        break;
                    case ActionType.REMOVE:
                        mutateType = _inventory.RemoveAmount(update.ItemID, update.Amount);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(update.Action.ToString());
                }

                Item item = _inventory.GetItem(update.ItemID);
                responses.Add(_inventoryUpdateResponseFactory.Create(item, update, mutateType));
            }

            _inventoryUpdateDispatcher.Dispatch(responses.ToArray());
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
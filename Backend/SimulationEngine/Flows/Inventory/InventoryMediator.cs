using IdelPog.Common.Enums;
using IdelPog.Messaging.Dispatch;
using IdelPog.SimulationEngine.Structures;

namespace IdelPog.SimulationEngine.Inventory
{
    /// <summary>
    /// See <see cref="IInventoryMediator"/> for documentation
    /// </summary>
    public class InventoryMediator : IInventoryMediator
    {
        private readonly IInventory _inventory;
        private readonly IItemFactory _itemFactory;
        private readonly IInventoryUpdateDTOFactory _inventoryUpdateDTOFactory;
        private readonly IDispatchMany<InventoryUpdateDTO> _inventoryUpdateDispatcher;

        public InventoryMediator(IInventory inventory, IItemFactory itemFactory, IInventoryUpdateDTOFactory inventoryUpdateDTOFactory, IDispatchMany<InventoryUpdateDTO>  inventoryUpdateDispatcher)
        {
            _inventory = inventory;
            _itemFactory = itemFactory;
            _inventoryUpdateDTOFactory = inventoryUpdateDTOFactory;
            _inventoryUpdateDispatcher = inventoryUpdateDispatcher;
        }
        
        public void UpdateInventory(IReadOnlyList<InventoryUpdate> updates)
        {
            List<InventoryUpdateDTO> updateDTOs = new(updates.Count);
            
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
                updateDTOs.Add(_inventoryUpdateDTOFactory.CreateInventoryUpdateDTO(item, update, mutateType));
            }
            
            _inventoryUpdateDispatcher.Dispatch(updateDTOs.ToArray());
        }

        /// <summary>
        /// Adds an amount to an <see cref="Item"/>, the <see cref="Item"/> will be found by using its connected <see cref="ItemID"/>
        /// </summary>
        /// <param name="itemID">The <see cref="Item"/> you want to add will have this <see cref="ItemID"/></param>
        /// <param name="amount">The amount you want to add</param>
        /// <remarks>If the <see cref="Item"/> with the passed <see cref="ItemID"/> is not found, it will be created</remarks>
        private MutateType CreateOrIncreaseAmount(ItemID itemID, int amount)
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
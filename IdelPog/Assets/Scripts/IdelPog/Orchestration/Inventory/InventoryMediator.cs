using System;
using IdelPog.Repository;
using IdelPog.Service;
using IdelPog.Structures;
using IdelPog.Structures.Enums;
using IdelPog.Structures.Item;

namespace IdelPog.Orchestration.Inventory
{
    /// <summary>
    /// See <see cref="IInventoryMediator"/> for documentation
    /// </summary>
    /// <seealso cref="CreateDefault"/>
    public class InventoryMediator : IInventoryMediator
    {
        private readonly IInventory _inventory;
        private readonly IItemFactory _itemFactory;
        
        public InventoryMediator(IInventory inventory)
        {
            _inventory = inventory;
        }

        /// <summary>
        /// Creates a <see cref="InventoryMediator"/> with all required dependencies
        /// </summary>
        /// <returns>A new <see cref="InventoryMediator"/> class with all dependencies resolved</returns>
        public static InventoryMediator CreateDefault()
        {
            IInventory repository = Repository.Inventory.CreateDefault();

            return new InventoryMediator(repository);
        }
        
        public ServiceResponse AddAmount(InventoryID inventoryID, int amount)
        {
           return HandleItemModification(inventoryID, amount, ActionType.ADD);
        }

        public ServiceResponse RemoveAmount(InventoryID inventoryID, int amount)
        {
            return HandleItemModification(inventoryID, amount, ActionType.REMOVE);
        }

        private ServiceResponse HandleItemModification(InventoryID inventoryID, int amount, ActionType action)
        {
            try
            {
                _inventory.RemoveAmount(inventoryID, amount);

                switch (action)
                {
                    case ActionType.ADD:
                        _inventory.AddAmount(inventoryID, amount);
                        break;
                    case ActionType.REMOVE:
                        _inventory.RemoveAmount(inventoryID, amount);
                        break;
                }
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }

            return ServiceResponse.Success();
        }
    }
}
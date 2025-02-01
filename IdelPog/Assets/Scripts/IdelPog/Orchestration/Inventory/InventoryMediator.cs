using System;
using IdelPog.Repository;
using IdelPog.Service;
using IdelPog.Structures;
using IdelPog.Structures.Item;

namespace IdelPog.Orchestration
{
    /// <summary>
    /// See <see cref="IInventoryMediator"/> for documentation
    /// </summary>
    /// <seealso cref="CreateDefault"/>
    public class InventoryMediator : IInventoryMediator
    {
        private readonly IInventory _inventory;
        private readonly IItemFactory _itemFactory;
        
        public InventoryMediator(IInventory inventory, IItemFactory itemFactory)
        {
            _inventory = inventory;
            _itemFactory = itemFactory;
        }

        /// <summary>
        /// Creates a <see cref="InventoryMediator"/> with all required dependencies
        /// </summary>
        /// <returns>A new <see cref="InventoryMediator"/> class with all dependencies resolved</returns>
        public static IInventoryMediator CreateDefault()
        {
            IInventory repository = Inventory.CreateDefault();
            IItemFactory itemFactory = new ItemFactory();

            return new InventoryMediator(repository, itemFactory);
        }
        
        public ServiceResponse AddAmount(InventoryID inventoryID, int amount)
        {
            try
            {
                if (_inventory.Contains(inventoryID) == false)
                {
                    // if an Item doesn't exist then we create one, and add it in
                    Item item = _itemFactory.CreateItem(inventoryID, amount);
                    _inventory.AddItem(item);
                }
                
                _inventory.AddAmount(inventoryID, amount);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }

            return ServiceResponse.Success();
        }

        public ServiceResponse RemoveAmount(InventoryID inventoryID, int amount)
        {
            if (_inventory.Contains(inventoryID) == false)
            {
                return ServiceResponse.Failure($"Error! Passed {inventoryID} does not exist!");
            }
            
            try
            {
                _inventory.RemoveAmount(inventoryID, amount);
            }
            catch (Exception exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }
            
            return ServiceResponse.Success();
        }
    }
}
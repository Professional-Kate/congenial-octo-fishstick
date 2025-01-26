using System;
using IdelPog.Repository;
using IdelPog.Structures;
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
        
        public ServiceResponse AddItem(InventoryID inventoryID, int amount)
        {
            throw new NotImplementedException();
        }

        public ServiceResponse RemoveItem(InventoryID inventoryID, int amount)
        {
            throw new NotImplementedException();
        }
    }
}
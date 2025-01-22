using System;
using IdelPog.Structures.Item;

namespace IdelPog.Repository
{
    /// <summary>
    /// The container class for all <see cref="Item"/>'s. See <see cref="IInventory"/> for documentation
    /// </summary>
    public sealed class Inventory : IInventory
    {
       private readonly Repository<InventoryID, Item> _inventory;

        public void AddAmount(InventoryID item, int amount)
        {
            throw new NotImplementedException();
        }

        public void RemoveAmount(InventoryID item, int amount)
        {
            throw new NotImplementedException();
        }

        public void AddItem(InventoryID item, int startingAmount)
        {
            throw new NotImplementedException();
        }

        public bool Contains(InventoryID item)
        {
            throw new NotImplementedException();
        }
    }
}
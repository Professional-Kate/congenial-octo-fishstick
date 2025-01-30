using System;
using IdelPog.Exceptions;
using IdelPog.Structures.Item;

namespace IdelPog.Repository
{
    /// <summary>
    /// Using this interface you can adjust the amount of any <see cref="Item"/> in the Inventory. 
    /// </summary>
    /// <seealso cref="AddAmount"/>
    /// <seealso cref="RemoveAmount"/>
    /// <seealso cref="AddItem"/>
    /// <seealso cref="Contains"/>
    public interface IInventory
    {
        /// <summary>
        /// Adds amount to an <see cref="Item"/>
        /// </summary>
        /// <param name="id">The <see cref="Item"/> you want to add to will have this <see cref="InventoryID"/></param>
        /// <param name="amount">The amount you want to add</param>
        /// <exception cref="ArgumentException">Will be thrown if the passed or amount is 0 or less</exception>
        /// <exception cref="NotFoundException">Will be thrown if the passed <see cref="Item"/> is not in the Inventory</exception>
        /// <exception cref="NotFoundException">Will be thrown if the passed <see cref="Item"/> is not in the Inventory</exception>
        public void AddAmount(InventoryID id, int amount);
        
        /// <summary>
        /// Remove an amount from an <see cref="Item"/> using its linked <see cref="InventoryID"/>
        /// </summary>
        /// <param name="id">The <see cref="Item"/> you want to remove an amount from will have this <see cref="InventoryID"/></param>
        /// <param name="amount">The amount you want to remove</param>
        /// <exception cref="ArgumentException">Will be thrown if the passed or amount is 0 or less</exception>
        /// <exception cref="ArgumentException">Will be thrown if the passed amount would cause the <see cref="Item"/>'s amount to be less than zero</exception>
        /// <exception cref="NotFoundException">Will be thrown if the passed <see cref="Item"/> is not in the Inventory</exception>
        public void RemoveAmount(InventoryID id, int amount);

        /// <summary>
        /// Adds a passed <see cref="Item"/> into the Inventory
        /// </summary>
        /// <param name="item">The <see cref="Item"/> you want to add</param>
        /// <exception cref="ArgumentException">Will be thrown if the passed <see cref="Item"/>'s amount is 0 or less</exception>
        /// <exception cref="ArgumentException">Will be thrown if the passed <see cref="InventoryID"/> already exists</exception>
        public void AddItem(Item item);
        
        /// <summary>
        /// Uses the passed <see cref="InventoryID"/> to see if an <see cref="Item"/> is within the Repository
        /// </summary>
        /// <param name="item">The <see cref="InventoryID"/> you want to ensure exists</param>
        public bool Contains(InventoryID item);
    }
}
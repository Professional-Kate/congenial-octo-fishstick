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
    public interface IInventory
    {
        /// <summary>
        /// Adds amount to an <see cref="Item"/>
        /// </summary>
        /// <param name="id">The <see cref="Item"/> you want to add to will have this <see cref="InventoryID"/></param>
        /// <param name="amount">The amount you want to add</param>
        /// <exception cref="ArgumentException">Will be thrown if the passed or amount is 0 or less</exception>
        /// <exception cref="NotFoundException">Will be thrown if the passed <see cref="Item"/> is not in the Inventory</exception>
        /// <remarks>This method will also create a new <see cref="Item"/> in the case it doesn't exist</remarks>
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
        /// Adds a new <see cref="Item"/> into the Inventory
        /// </summary>
        /// <param name="id">The <see cref="Item"/> you want to add will have this <see cref="InventoryID"/></param>
        /// <param name="startingAmount">The amount the item should start with</param>
        /// <exception cref="ArgumentException">Will be thrown if the passed or amount is 0 or less</exception>
        /// <exception cref="ArgumentException">Will be thrown if the passed <see cref="InventoryID"/> already exists</exception>
        public void AddItem(InventoryID id, int startingAmount);
    }
}
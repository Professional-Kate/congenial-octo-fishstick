using IdelPog.Structures;
using IdelPog.Structures.Item;

namespace IdelPog.Orchestration.Inventory
{
    /// <summary>
    /// Handles Adding and Removing an amount from an <see cref="Item"/>. Add, and Remove will both create and remove an <see cref="Item"/> respectively
    /// </summary>
    /// <seealso cref="AddAmount"/>
    /// <seealso cref="RemoveAmount"/>
    public interface IInventoryMediator
    {
        /// <summary>
        /// Adds an amount to an <see cref="Item"/>, the <see cref="Item"/> will be found by using its connected <see cref="InventoryID"/>
        /// </summary>
        /// <param name="inventoryID">The <see cref="Item"/> you want to add will have this <see cref="InventoryID"/></param>
        /// <param name="amount">The amount you want to add</param>
        /// <returns>A <see cref="ServiceResponse"/> object that tells you how the operation went</returns>
        public ServiceResponse AddAmount(InventoryID inventoryID, int amount);
        
        /// <summary>
        /// Removes an amount from an <see cref="Item"/>, the <see cref="Item"/> will be found by using its connected <see cref="InventoryID"/>
        /// </summary>
        /// <param name="inventoryID">The <see cref="Item"/> you want to remove amount from will have this <see cref="InventoryID"/></param>
        /// <param name="amount">The amount you want to remove</param>
        /// <returns>A <see cref="ServiceResponse"/> object that tells you how the operation went</returns>
        public ServiceResponse RemoveAmount(InventoryID inventoryID, int amount);
    }
}
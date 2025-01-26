using IdelPog.Structures.Item;

namespace IdelPog.Repository.Inventory
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
        /// 
        /// </summary>
        /// <returns></returns>
        public IInventory CreateDefault();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public void AddAmount(InventoryID item, int amount);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="amount"></param>
        public void RemoveAmount(InventoryID item, int amount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="startingAmount"></param>
        public void AddItem(InventoryID item, int startingAmount);
    }
}
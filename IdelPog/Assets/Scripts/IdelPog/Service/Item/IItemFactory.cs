using IdelPog.Structures.Item;

namespace IdelPog.Service
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="CreateItem"/>
    public interface IItemFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="startingAmount"></param>
        /// <returns></returns>
        public Item CreateItem(InventoryID id, int startingAmount); 
    }
}
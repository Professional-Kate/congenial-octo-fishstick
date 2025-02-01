using IdelPog.Structures;
using IdelPog.Structures.Item;

namespace IdelPog.Service
{
    /// <summary>
    /// This interface will create a new <see cref="Item"/> using the passed arguments
    /// </summary>
    /// <seealso cref="CreateItem"/>
    public interface IItemFactory
    {
        /// <summary>
        /// Creates a new <see cref="Item"/>
        /// </summary>
        /// <param name="id">The <see cref="InventoryID"/> the created <see cref="Item"/> has</param>
        /// <param name="information"><see cref="Information"/></param>
        /// <param name="sellPrice">The sell price the <see cref="Item"/> should have</param>
        /// <param name="startingAmount">The starting amount of this <see cref="Item"/></param>
        /// <returns>A newly created <see cref="Item"/></returns>
        /// <remarks>This method will assert that each passed argument is valid</remarks>
        public Item CreateItem(InventoryID id, Information information, int sellPrice, int startingAmount); 
    }
}
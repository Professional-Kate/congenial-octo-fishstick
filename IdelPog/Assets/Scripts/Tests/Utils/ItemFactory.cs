using IdelPog.Constants;
using IdelPog.Structures.Models.Item;

namespace Tests.Utils
{
    internal abstract class ItemFactory
    {
        internal static Item CreateOakWood()
        {
            return new Item(InventoryID.OAK_WOOD, ItemConstants.OAK_WOOD, 1, 0);
        }
    }
}
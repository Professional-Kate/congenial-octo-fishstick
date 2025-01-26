using IdelPog.Constants;
using IdelPog.Structures.Item;

namespace IdelPog.Service
{
    public class ItemFactory : IItemFactory
    {
        public Item CreateItem(InventoryID id, int startingAmount)
        {
            // TODO: This class will need to be able to find the relevant Information object and sell price for the item. For now, they will be placeholders
            return new Item(id, ItemConstants.PLACE_HOLDER, 1, startingAmount);
        }
    }
}
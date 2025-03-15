using IdelPog.Constants;
using IdelPog.Structures.Builders;
using IdelPog.Structures.Models.Item;

namespace Tests.Utils
{
    internal abstract class ItemFactory
    {
        internal static Item CreateOakWood()
        {
            return ItemBuilder.Builder()
                .InventoryID(InventoryID.OAK_WOOD)
                .Information(ItemConstants.OAK_WOOD)
                .SellPrice(1)
                .Amount(1)
                .Build();
        }
    }
}
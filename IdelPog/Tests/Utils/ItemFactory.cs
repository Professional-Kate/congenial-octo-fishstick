using IdelPogTemp.Main.Constants;
using IdelPogTemp.Main.Structures.Models.Builders.Item;
using IdelPogTemp.Main.Structures.Models.Item;

namespace IdelPogTemp.Tests.Utils
{
    internal abstract class ItemFactory
    {
        internal static Item CreateOakWood()
        {
            return ItemBuilder.Builder()
                .InventoryID(InventoryID.OAK_WOOD)
                .Information(ItemConstants.OAK_WOOD)
                .SellPrice(1)
                .Amount(0)
                .Build();
        }
    }
}
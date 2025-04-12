using IdelPog.Engine.Constants;
using IdelPog.Engine.Structures;
using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Utilities.Builders.Item;

namespace IdelPog.Tests.Utils
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
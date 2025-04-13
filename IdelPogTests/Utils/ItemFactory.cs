using IdelPog.Engine.Constants;
using IdelPog.Engine.Models;
using IdelPog.Engine.Structures.Enums;

namespace IdelPogTests.Utils
{
    internal abstract class ItemFactory
    {
        internal static Item CreateOakWood()
        {
            return ItemBuilder.Create(InventoryID.OAK_WOOD, ItemConstants.OAK_WOOD)
                .SellPrice(1)
                .Amount(0)
                .Build();
        }
    }
}
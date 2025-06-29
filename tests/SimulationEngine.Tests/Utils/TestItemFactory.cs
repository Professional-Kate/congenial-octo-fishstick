using IdelPog.SimulationEngine.Constants;
using IdelPog.SimulationEngine.Inventory;

namespace IdelPogTests.Utils
{
    internal abstract class TestItemFactory
    {
        internal static Item CreateOakWood()
        {
            return ItemBuilder.Create(ItemID.OAK_WOOD, ItemConstants.OAK_WOOD)
                .SellPrice(1)
                .Amount(0)
                .Build();
        }
    }
}
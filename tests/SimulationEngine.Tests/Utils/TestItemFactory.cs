using IdelPog.SimulationEngine.Inventory;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPogTests.Utils
{
    internal abstract class TestItemFactory
    {
        internal static Item CreateOakWood()
        {
            return ItemBuilder.Create(ItemID.OAK_WOOD, new Information { Description = "Oak Wood", Name = "Oak, Wood." })
                .SellPrice(1)
                .Amount(0)
                .Build();
        }
    }
}
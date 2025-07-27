using IdelPog.SimulationEngine.Inventory;
using IdelPog.SimulationEngine.Models;

namespace IdelPogTests.Utils
{
    internal abstract class TestItemFactory
    {
        internal static Item CreateOakWood()
        {
            return new Item(ItemID.OAK_WOOD, 0, new Information { Description = "Oak Wood", Name = "Oak, Wood." }, 0);
        }
    }
}
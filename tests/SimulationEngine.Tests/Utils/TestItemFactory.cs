using IdelPog.SimulationEngine.Inventory;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPogTests.Utils
{
    internal abstract class TestItemFactory
    {
        internal static Item CreateOakWood()
        {
            return new Item(ItemID.OAK_WOOD, 0, 1, new Information { Description = "Oak Wood", Name = "Oak, Wood." });
        }
    }
}
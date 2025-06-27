using IdelPog.SimulationEngine.Constants;
using IdelPog.SimulationEngine.Flows.Inventory;
using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Structures.Enums;

namespace IdelPogTests.Utils
{
    internal abstract class ItemFactory
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
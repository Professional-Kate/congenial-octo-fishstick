using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information.Contracts;
using IdelPog.Inventory.Contracts;

namespace IdelPog.Inventory.Tests
{
    internal static class ItemFactory
    {
        internal static Item CreateOakWood()
        {
            return new Item(ItemID.STONE, 0, new Information { Description = "Oak Wood", Name = "Oak, Wood." }, 0);
        }
    }
}
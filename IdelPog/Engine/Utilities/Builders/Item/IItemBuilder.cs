using IdelPog.Engine.Structures;
using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Types;

namespace IdelPog.Engine.Utilities.Builders
{
    /// <summary>
    /// Builds a new <see cref="Item"/>
    /// </summary>
    /// <seealso cref="InventoryID"/>
    /// <seealso cref="Information"/>
    /// <seealso cref="SellPrice"/>
    /// <seealso cref="Amount"/>
    /// <seealso cref="Build"/>
    public interface IItemBuilder
    {
        public IItemBuilder InventoryID(InventoryID inventoryID);

        public IItemBuilder Information(Information information);

        public IItemBuilder SellPrice(int sellPrice);

        public IItemBuilder Amount(int amount);

        public Item Build();
    }
}
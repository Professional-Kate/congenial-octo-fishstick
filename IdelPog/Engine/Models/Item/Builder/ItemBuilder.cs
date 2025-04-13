using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Types;

namespace IdelPog.Engine.Models
{
    /// <inheritdoc cref="IItemBuilder"/>
    public sealed class ItemBuilder : IItemBuilder
    {
        private readonly InventoryID _inventoryID;
        private readonly Information _information;
        private int _sellPrice { get; set; }
        private int _amount { get; set; }

        private ItemBuilder(InventoryID inventoryID, Information information, int sellPrice, int amount)
        {
            _inventoryID = inventoryID;
            _information = information;
            _sellPrice = sellPrice;
            _amount = amount;
        }

        /// <summary>
        /// Constructs a safe ItemBuilder, which can be called to <see cref="Build"/> right after
        /// </summary>
        /// <param name="id">The ID of the wanted <see cref="Item"/></param>
        /// <param name="information"></param>
        public static IItemBuilder Create(InventoryID id, Information information)
        {
            return new ItemBuilder(id, information, 10, 1);
        }

        public IItemBuilder SellPrice(int sellPrice)
        {
            _sellPrice = sellPrice;
            
            return this;
        }

        public IItemBuilder Amount(int amount)
        {
            _amount = amount;
            
            return this;
        }

        public Item Build()
        {
            return new Item(_inventoryID, _information, _sellPrice, _amount);
        }
    }
}
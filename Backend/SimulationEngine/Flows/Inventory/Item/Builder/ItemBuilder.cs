using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Inventory
{
    /// <inheritdoc cref="IItemBuilder"/>
    public sealed class ItemBuilder : IItemBuilder
    {
        private readonly ItemID _itemID;
        private readonly Information _information;
        private uint _sellPrice { get; set; }
        private uint _amount { get; set; }

        private ItemBuilder(ItemID itemID, Information information, uint sellPrice, uint amount)
        {
            _itemID = itemID;
            _information = information;
            _sellPrice = sellPrice;
            _amount = amount;
        }

        /// <summary>
        /// Constructs a safe ItemBuilder, which can be called to <see cref="Build"/> right after
        /// </summary>
        /// <param name="id">The ID of the wanted <see cref="Item"/></param>
        /// <param name="information">The information of the wanted <see cref="Item"/></param>
        /// <remarks>Will assign default amounts to all properties</remarks>
        public static IItemBuilder Create(ItemID id, Information information)
        {
            return new ItemBuilder(id, information, 1, 1);
        }

        public IItemBuilder SellPrice(uint sellPrice)
        {
            _sellPrice = sellPrice;

            return this;
        }

        public IItemBuilder Amount(uint amount)
        {
            _amount = amount;

            return this;
        }

        public Item Build()
        {
            return new Item(_itemID, _information, _sellPrice, _amount);
        }
    }
}
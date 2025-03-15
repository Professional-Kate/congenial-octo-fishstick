using IdelPog.Structures.Models.Item;

namespace IdelPog.Structures.Builders
{
    /// <inheritdoc cref="IItemBuilder"/>
    public class ItemBuilder : IItemBuilder
    {
        private InventoryID _inventoryID { get; set; }
        private Information _information { get; set; }
        private int _sellPrice { get; set; }
        private int _amount { get; set; }
        
        public static IItemBuilder Builder() => new ItemBuilder();

        public IItemBuilder InventoryID(InventoryID inventoryID)
        {
            _inventoryID = inventoryID;

            return this;
        }

        public IItemBuilder Information(Information information)
        {
            _information = information;
            
            return this;
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
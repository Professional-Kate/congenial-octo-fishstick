using IdelPog.Structures.Models.Item;

namespace IdelPog.Structures.Builders
{
    public interface IItemBuilder
    {
        public IItemBuilder InventoryID(InventoryID inventoryID);

        public IItemBuilder Information(Information information);

        public IItemBuilder SellPrice(int sellPrice);

        public IItemBuilder Amount(int amount);

        public Item Build();
    }
}
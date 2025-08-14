using IdelPog.Core.Contracts.Enum;
using IdelPog.Inventory.Contracts;

namespace IdelPog.Inventory.Factory.Interface
{
    public interface IItemFactory
    {
        public Item CreateItem(ItemID itemID, uint amount);
    }
}
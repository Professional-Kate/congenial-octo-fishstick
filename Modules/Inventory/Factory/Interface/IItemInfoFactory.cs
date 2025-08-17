using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Inventory.Factory.Interface
{
    public interface IItemInfoFactory
    {
        public ItemInfo Create(ItemID itemID, uint sellPrice, uint amount);
    }
}
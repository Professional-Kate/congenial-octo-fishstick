using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information.Contracts;

namespace IdelPog.Inventory.Factory.Interface
{
    public interface IItemInfoFactory
    {
        public ItemInfo Create(ItemID itemID, uint sellPrice, uint amount, Information information);
    }
}
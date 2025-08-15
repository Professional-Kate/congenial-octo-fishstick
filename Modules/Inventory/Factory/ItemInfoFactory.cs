using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Inventory.Factory.Interface;

namespace IdelPog.Inventory.Factory
{
    public class ItemInfoFactory : IItemInfoFactory
    {
        public ItemInfo Create(ItemID itemID, uint sellPrice, uint amount)
        {
            return new ItemInfo
            {
                ItemID = itemID,
                BaseSellPrice = sellPrice,
                Amount = amount
            };
        }
    }
}
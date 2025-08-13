using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Information;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Factory.Interface;

namespace IdelPog.Inventory.Factory
{
    public class ItemFactory : IItemFactory
    {
        private readonly IMapper<ItemID> _itemMapper;

        public ItemFactory(IMapper<ItemID> itemMapper)
        {
            _itemMapper = itemMapper;
        }

        public Item CreateItem(ItemID itemID, uint amount)
        {
            // TODO: need a way to get sell price
            return new Item(itemID, 1, _itemMapper.GetInformation(itemID), amount);
        }
    }
}
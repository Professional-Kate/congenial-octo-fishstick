using IdelPog.Constants;
using IdelPog.Structures.Item;

namespace IdelPog.Service
{
    public class ItemFactory : IItemFactory, ICreateDefault
    {
        private readonly IMapper<InventoryID> _itemMapper;

        public ItemFactory(IMapper<InventoryID> itemMapper)
        {
            _itemMapper = itemMapper;
        }

        public static IItemFactory CreateDefault()
        {
            IMapper<InventoryID> itemMapper = new Mapper<InventoryID>();
            
            return new ItemFactory(itemMapper);
        }

        public Item CreateItem(InventoryID id, int startingAmount)
        {
            // TODO: This class will need to be able to find the relevant Information object and sell price for the item. For now, they will be placeholders
            return new Item(id, ItemConstants.PLACE_HOLDER, 1, startingAmount);
        }
    }
}
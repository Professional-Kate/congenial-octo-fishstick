using IdelPog.SimulationEngine.Models;
using IdelPog.SimulationEngine.Service;

namespace IdelPog.SimulationEngine.Inventory
{
    public class ItemFactory(IMapper<ItemID> itemMapper) : IItemFactory
    {
        public Item CreateItem(ItemID itemID, uint amount)
        {
            return new Item(itemID, 1, itemMapper.GetInformation(itemID), amount);
        }
    }
}
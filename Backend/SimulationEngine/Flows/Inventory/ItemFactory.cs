using IdelPog.SimulationEngine.Service;

namespace IdelPog.SimulationEngine.Inventory
{
    public class ItemFactory(IMapper<ItemID> itemMapper) : IItemFactory
    {
        public Item CreateItem(ItemID itemID, int amount)
        {
            return ItemBuilder
                .Create(itemID, itemMapper.GetInformation(itemID))
                .Amount(amount)
                .Build();
        }
    }
}
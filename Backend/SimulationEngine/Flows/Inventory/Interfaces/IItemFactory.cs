namespace IdelPog.SimulationEngine.Inventory
{
    public interface IItemFactory
    {
        public Item CreateItem(ItemID itemID, int amount);
    }
}
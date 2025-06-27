namespace IdelPog.SimulationEngine.Flows.Inventory
{
    public interface IItemFactory
    {
        public Item CreateItem(ItemID itemID, int amount);
    }
}
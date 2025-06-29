namespace IdelPog.SimulationEngine.Inventory
{
    public interface IInventoryMediator
    {
        public void UpdateInventory(IReadOnlyList<InventoryUpdate> updates);
    }
}
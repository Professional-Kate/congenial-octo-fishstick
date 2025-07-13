namespace IdelPog.SimulationEngine.Inventory
{
    public interface IInventoryController
    {
        public void UpdateInventory(IReadOnlyList<InventoryUpdate> updates);
    }
}
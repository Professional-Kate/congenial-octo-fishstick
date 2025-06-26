namespace IdelPog.SimulationEngine.Flows.Inventory
{
    public interface IInventoryController
    {
        public void UpdateInventory(IReadOnlyList<InventoryUpdate> updates);
    }
}
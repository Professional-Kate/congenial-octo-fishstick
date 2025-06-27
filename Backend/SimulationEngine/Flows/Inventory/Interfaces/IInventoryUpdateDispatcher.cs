namespace IdelPog.SimulationEngine.Flows.Inventory
{
    public interface IInventoryUpdateDispatcher
    {
        public void DispatchUpdates(InventoryUpdateDTO[] inventoryUpdates);
    }
}
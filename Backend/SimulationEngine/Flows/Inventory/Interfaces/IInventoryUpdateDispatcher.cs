namespace IdelPog.SimulationEngine.Inventory
{
    public interface IInventoryUpdateDispatcher
    {
        public void DispatchUpdates(InventoryUpdateDTO[] inventoryUpdates);
    }
}
using IdelPog.Messaging.Listeners;

namespace IdelPog.SimulationEngine.Flows.Inventory
{
    public class InventoryUpdateListener(IInventoryController inventoryController) : IBufferListener<InventoryUpdate>
    {
        public Type ListenerType { get; } =  typeof(InventoryUpdate);
        
        public void Handle(IReadOnlyList<InventoryUpdate> buffer)
        {
            inventoryController.UpdateInventory(buffer);
        }
    }
}
using IdelPog.Messaging.Listeners.Buffer;

namespace IdelPog.SimulationEngine.Inventory
{
    public class InventoryController : IBatchController<InventoryUpdate>
    {
        private readonly IInventoryMediator _inventoryMediator;

        public InventoryController(IInventoryMediator inventoryMediator)
        {
            _inventoryMediator = inventoryMediator;
        }
        public void HandleMessages(IReadOnlyList<InventoryUpdate> messages)
        {
            _inventoryMediator.UpdateInventory(messages);
        }
    }
}
namespace IdelPog.SimulationEngine.Inventory
{
    public class InventoryController : IInventoryController
    {
        private readonly IInventoryMediator _inventoryMediator;

        public InventoryController(IInventoryMediator inventoryMediator)
        {
            _inventoryMediator = inventoryMediator;
        }

        public void UpdateInventory(IReadOnlyList<InventoryUpdate> updates)
        {
            _inventoryMediator.UpdateInventory(updates);
        }
    }
}
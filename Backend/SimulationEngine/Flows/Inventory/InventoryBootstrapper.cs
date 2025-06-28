using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Service;

namespace IdelPog.SimulationEngine.Flows.Inventory
{
    public class InventoryBootstrapper
    {
        public void Initialize(IBufferMessenger bufferMessenger, IBufferManager bufferManager)
        {
            IInventoryUpdateDispatcher inventoryUpdateDispatcher = new InventoryUpdateDispatcher(bufferManager);
            IInventoryUpdateDTOFactory inventoryUpdateDTOFactory = new InventoryUpdateDTOFactory();
            IMapper<ItemID> itemMapper = new Mapper<ItemID>();
            IItemFactory itemFactory = new ItemFactory(itemMapper);
            IInventory inventory = new Inventory();
            IInventoryMediator inventoryMediator = new InventoryMediator(inventory, itemFactory, inventoryUpdateDTOFactory, inventoryUpdateDispatcher);
            
            IInventoryController inventoryController = new InventoryController(inventoryMediator);
            InventoryUpdateListener inventoryUpdateListener = new(inventoryController);
            
            bufferMessenger.Subscribe(inventoryUpdateListener);
        }
    }
}
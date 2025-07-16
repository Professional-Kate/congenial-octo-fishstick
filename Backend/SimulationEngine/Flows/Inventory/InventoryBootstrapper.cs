using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Messenger;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Service;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.SimulationEngine.Inventory
{
    public class InventoryBootstrapper
    {
        public void Initialize(IBufferMessenger bufferMessenger, IBufferManager bufferManager)
        {
            IAssertNotNull assertNotNull = new AssertNotNull(new ThrowHandler());
            IAssertCollectionNotEmpty assertCollectionNotEmpty = new AssertCollectionNotEmpty(new ThrowHandler());
            
            IDispatchMany<InventoryUpdateDTO> inventoryUpdateDispatcher = new ManagedDispatcher<InventoryUpdateDTO>(bufferManager, assertNotNull, assertCollectionNotEmpty);
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
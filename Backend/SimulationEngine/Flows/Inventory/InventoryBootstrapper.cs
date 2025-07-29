using IdelPog.Common.Factories;
using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Dispatch;
using IdelPog.Messaging.Listeners;
using IdelPog.Messaging.Listeners.Buffer;
using IdelPog.Messaging.Messenger;
using IdelPog.Messaging.Orchestration;
using IdelPog.SimulationEngine.Service;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.SimulationEngine.Inventory
{
    public class InventoryBootstrapper
    {
        public void Initialize(IBufferMessenger bufferMessenger, IBufferManager bufferManager)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(new ThrowHandler());
            ICollectionAssertion collectionAssertion = new CollectionAssertion(new ThrowHandler());

            IDispatchMany<InventoryUpdateDTO> inventoryUpdateDispatcher = new ManagedDispatcher<InventoryUpdateDTO>(bufferManager, objectNullAssertion, collectionAssertion);

            IInventoryUpdateDTOFactory inventoryUpdateDTOFactory = new InventoryUpdateDTOFactory();
            IMapper<ItemID> itemMapper = new Mapper<ItemID>();
            IItemFactory itemFactory = new ItemFactory(itemMapper);
            IInventory inventory = new Inventory();
            IInventoryMediator inventoryMediator = new InventoryMediator(inventory, itemFactory, inventoryUpdateDTOFactory, inventoryUpdateDispatcher);

            IErrorDTOFactory errorDTOFactory = new ErrorDTOFactory();
            IErrorFactory<InventoryUpdateErrorDTO, IReadOnlyList<InventoryUpdate>> inventoryUpdateErrorFactory = new InventoryUpdateErrorDTOFactory(errorDTOFactory);
            IDispatchOne<InventoryUpdateErrorDTO> updateErrorDispatcher = new ManagedDispatcher<InventoryUpdateErrorDTO>(bufferManager, objectNullAssertion, collectionAssertion);
            IContextualHandler<IReadOnlyList<InventoryUpdate>> updateDispatchHandler = new DispatchingHandler<InventoryUpdateErrorDTO, IReadOnlyList<InventoryUpdate>>(updateErrorDispatcher, inventoryUpdateErrorFactory);
            IBatchControllerExecutionAssertion<InventoryUpdate> updateExecutionAssertion = new BatchControllerExecutionAssertion<InventoryUpdate>(updateDispatchHandler);
            IBatchController<InventoryUpdate> inventoryController = new InventoryController(inventoryMediator);
            IBufferListener<InventoryUpdate> inventoryUpdateListener = new ManagedBufferListener<InventoryUpdate>(inventoryController, updateExecutionAssertion);

            bufferMessenger.Subscribe(inventoryUpdateListener);
        }
    }
}
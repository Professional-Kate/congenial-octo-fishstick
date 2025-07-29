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

            IDispatchMany<InventoryUpdateResponse> inventoryUpdateDispatcher = new ManagedDispatcher<InventoryUpdateResponse>(bufferManager, objectNullAssertion, collectionAssertion);

            IInventoryUpdateResponseFactory inventoryUpdateResponseFactory = new InventoryUpdateResposneFactory();
            IMapper<ItemID> itemMapper = new Mapper<ItemID>();
            IItemFactory itemFactory = new ItemFactory(itemMapper);
            IInventory inventory = new Inventory();
            IInventoryMediator inventoryMediator = new InventoryMediator(inventory, itemFactory, inventoryUpdateResponseFactory, inventoryUpdateDispatcher);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<InventoryUpdateError, IReadOnlyList<InventoryUpdate>> inventoryUpdateErrorFactory = new InventoryUpdateErrorFactory(baseErrorFactory);
            IDispatchOne<InventoryUpdateError> updateErrorDispatcher = new ManagedDispatcher<InventoryUpdateError>(bufferManager, objectNullAssertion, collectionAssertion);
            IContextualHandler<IReadOnlyList<InventoryUpdate>> updateDispatchHandler = new DispatchingHandler<InventoryUpdateError, IReadOnlyList<InventoryUpdate>>(updateErrorDispatcher, inventoryUpdateErrorFactory);
            IBatchControllerExecutionAssertion<InventoryUpdate> updateExecutionAssertion = new BatchControllerExecutionAssertion<InventoryUpdate>(updateDispatchHandler);
            IBatchController<InventoryUpdate> inventoryController = new InventoryController(inventoryMediator);
            IBufferListener<InventoryUpdate> inventoryUpdateListener = new ManagedBufferListener<InventoryUpdate>(inventoryController, updateExecutionAssertion);

            bufferMessenger.Subscribe(inventoryUpdateListener);
        }
    }
}
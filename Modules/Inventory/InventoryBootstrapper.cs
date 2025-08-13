using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Factory;
using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Information;
using IdelPog.Core.Messaging.Assertion;
using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Controller;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Messaging.Messenger;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Factory;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Mediator;
using IdelPog.Inventory.Service;

namespace IdelPog.Inventory
{
    public class InventoryBootstrapper
    {
        public static void Initialize(IBufferMessenger bufferMessenger, IBufferManager bufferManager)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);

            IDispatchMany<InventoryUpdateResponse> inventoryUpdateDispatcher = new ManagedDispatcher<InventoryUpdateResponse>(bufferManager, objectNullAssertion, collectionAssertion);

            IStateRepository<ItemID, Item> itemRepository = new StateRepository<ItemID, Item>();
            IInventoryUpdateResponseFactory inventoryUpdateResponseFactory = new InventoryUpdateResponseFactory();
            IMapper<ItemID> itemMapper = new Mapper<ItemID>(foundAssertion, uniqueAssertion);
            IItemFactory itemFactory = new ItemFactory(itemMapper);
            IInventory inventory = new Service.Inventory(itemRepository, foundAssertion, uniqueAssertion);
            IBatchMediator<InventoryUpdate> inventoryMediator = new InventoryMediator(inventory, itemFactory, inventoryUpdateResponseFactory, inventoryUpdateDispatcher);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<InventoryUpdateError, IReadOnlyList<InventoryUpdate>> inventoryUpdateErrorFactory = new InventoryUpdateErrorFactory(baseErrorFactory);
            IDispatchOne<InventoryUpdateError> updateErrorDispatcher = new ManagedDispatcher<InventoryUpdateError>(bufferManager, objectNullAssertion, collectionAssertion);
            IContextualHandler<IReadOnlyList<InventoryUpdate>> updateDispatchHandler = new DispatchingHandler<InventoryUpdateError, IReadOnlyList<InventoryUpdate>>(updateErrorDispatcher, inventoryUpdateErrorFactory);
            IBatchControllerExecutionAssertion<InventoryUpdate> updateExecutionAssertion = new BatchControllerExecutionAssertion<InventoryUpdate>(updateDispatchHandler);
            IBatchController<InventoryUpdate> inventoryController = new ManagedBatchController<InventoryUpdate>(inventoryMediator);
            IBufferListener<InventoryUpdate> inventoryUpdateListener = new ManagedBufferListener<InventoryUpdate>(inventoryController, updateExecutionAssertion);

            bufferMessenger.Subscribe(inventoryUpdateListener);
        }
    }
}
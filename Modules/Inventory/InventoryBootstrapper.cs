using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Factory;
using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Flows.Registry;
using IdelPog.Core.Information;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Controller;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Single;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Inventory.Assertion;
using IdelPog.Inventory.Assertion.Interface;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Factory;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Mediator;
using IdelPog.Inventory.Service;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory
{
    public static class InventoryBootstrapper
    {
        /// <summary>
        /// Registers the InventoryUpdate flow
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="flowRegister">Used to register the InventoryUpdate flow</param>
        /// <remarks>
        /// Listens to -> <see cref="InventoryUpdate"/>. On Success -> <see cref="InventoryUpdateResponse"/>. On Error -> <see cref="InventoryUpdateError"/>
        /// </remarks>
        public static void RegisterInventoryUpdate(IBufferManager bufferManager, IBatchRegister flowRegister)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);
            IAmountAssertion amountAssertion = new AmountAssertion(throwHandler);

            IDispatchOne<InventoryUpdateResponse> inventoryUpdateDispatcher = new ManagedDispatcher<InventoryUpdateResponse>(bufferManager, objectNullAssertion, collectionAssertion);

            IMapper<ItemID> itemMapper = new Mapper<ItemID>(foundAssertion, uniqueAssertion);
            itemMapper.AddInformation(ItemID.STONE, new Information { Description = "Rock and...", Name = "Stone" });
            itemMapper.AddInformation(ItemID.COPPER, new Information { Description = "It's like less cool bronze", Name = "Copper" });
            itemMapper.AddInformation(ItemID.IRON, new Information { Description = "Your job is to mine Diamonds", Name = "Iron" });
            itemMapper.AddInformation(ItemID.GOLD, new Information { Description = "It's like less cool copper", Name = "Gold" });
            
            IStateRepository<ItemID, Item> itemRepository = new StateRepository<ItemID, Item>();
            IInventoryUpdateResponseFactory inventoryUpdateResponseFactory = new InventoryUpdateResponseFactory();
            IItemFactory itemFactory = new ItemFactory(itemMapper);
            IInventoryUpdateSummarizer summarizer = new InventoryUpdateSummarizer();
            IItemInfoFactory itemInfoFactory = new ItemInfoFactory();
            IInventoryUpdateEntryFactory inventoryUpdateEntryFactory = new InventoryUpdateEntryFactory();
            IInventory inventory = new Service.Inventory(itemRepository, foundAssertion, uniqueAssertion, amountAssertion);
            IBatchMediator<InventoryUpdate> inventoryMediator = new InventoryUpdateMediator(inventory, itemFactory, summarizer, inventoryUpdateResponseFactory, itemInfoFactory, inventoryUpdateEntryFactory, inventoryUpdateDispatcher, collectionAssertion);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<InventoryUpdateError, IReadOnlyList<InventoryUpdate>> inventoryUpdateErrorFactory = new InventoryUpdateErrorFactory(baseErrorFactory);
            IBatchController<InventoryUpdate> inventoryController = new ManagedBatchController<InventoryUpdate>(inventoryMediator);
            
            flowRegister.Register(inventoryController, inventoryUpdateErrorFactory);
        }
    }
}
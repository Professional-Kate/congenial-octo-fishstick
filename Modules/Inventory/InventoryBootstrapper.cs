using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Factory;
using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Flows.Registry;
using IdelPog.Core.Information;
using IdelPog.Core.Information.Contracts;
using IdelPog.Core.Logging;
using IdelPog.Core.Logging.Writer;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Controller;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Inventory.Assertion;
using IdelPog.Inventory.Assertion.Interface;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Contracts.Error;
using IdelPog.Inventory.Contracts.Response;
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

            ILogWriter writer = new ConsoleWriter();
            IBufferLogger bufferLogger = new BufferLoggingService(writer);
            
            IDispatchMany<InventoryUpdateResponse> inventoryUpdateDispatcher = new ManagedDispatcher<InventoryUpdateResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);

            IMapper<ItemID> itemMapper = new Mapper<ItemID>(foundAssertion, uniqueAssertion);
            itemMapper.AddInformation(ItemID.STONE, new Information { Description = "Rock and...", Name = "Stone" });
            itemMapper.AddInformation(ItemID.COPPER, new Information { Description = "It's like less cool bronze", Name = "Copper" });
            itemMapper.AddInformation(ItemID.IRON, new Information { Description = "Your job is to mine Diamonds", Name = "Iron" });
            itemMapper.AddInformation(ItemID.GOLD, new Information { Description = "It's like less cool copper", Name = "Gold" });
            itemMapper.AddInformation(ItemID.DIAMOND, new Information { Description = "Fancy coal", Name = "Diamond" });
            itemMapper.AddInformation(ItemID.EMERALD, new Information { Description = "Your villagers will love this", Name = "Emerald" });
            itemMapper.AddInformation(ItemID.RUBY, new Information { Description = "Evil Diamond (thus worth more)", Name = "Ruby" });
            itemMapper.AddInformation(ItemID.OAK, new Information { Description = "The most basic of woods", Name = "Oak" });
            itemMapper.AddInformation(ItemID.SPRUCE, new Information { Description = "Ohhhh now we got something good", Name = "Spruce" });
            itemMapper.AddInformation(ItemID.BIRCH, new Information { Description = "I like the colours", Name = "Birch" });
            itemMapper.AddInformation(ItemID.HERBS, new Information { Description = "Delicious herbs (do not smoke them)", Name = "Herbs" });
            itemMapper.AddInformation(ItemID.SMALL_INSECTS, new Information { Description = "Could use these for fishing...", Name = "Small Insects" });
            itemMapper.AddInformation(ItemID.HONEY, new Information { Description = "Delicious and it was only slightly painful", Name = "Honey" });
            itemMapper.AddInformation(ItemID.WATER, new Information { Description = "Finally learnt how to collect water?", Name = "Water" });
            itemMapper.AddInformation(ItemID.SAND, new Information { Description = "It gets everywhere...", Name = "Sand" });
            
            IInventoryUpdateFactory updateFactory = new InventoryUpdateFactory();
            IStateRepository<ItemID, Item> itemRepository = new StateRepository<ItemID, Item>();
            IInventoryUpdateResponseFactory inventoryUpdateResponseFactory = new InventoryUpdateResponseFactory();
            IItemFactory itemFactory = new ItemFactory(itemMapper);
            IInventoryUpdateSummarizer summarizer = new InventoryUpdateSummarizer(updateFactory, collectionAssertion);
            IItemInfoFactory itemInfoFactory = new ItemInfoFactory();
            IInventory inventory = new Service.Inventory(itemRepository, foundAssertion, uniqueAssertion, amountAssertion);
            IBatchMediator<InventoryUpdate> inventoryMediator = new InventoryUpdateMediator(inventory, itemFactory, summarizer, inventoryUpdateResponseFactory, itemInfoFactory, itemMapper, inventoryUpdateDispatcher, collectionAssertion);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<InventoryUpdateError, IReadOnlyList<InventoryUpdate>> inventoryUpdateErrorFactory = new InventoryUpdateErrorFactory(baseErrorFactory);
            IBatchController<InventoryUpdate> inventoryController = new ManagedBatchController<InventoryUpdate>(inventoryMediator);
            
            flowRegister.RegisterBatch(inventoryController, inventoryUpdateErrorFactory);
        }
    }
}
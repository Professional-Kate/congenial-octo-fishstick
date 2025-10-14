using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Factory;
using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Flows.Registry;
using IdelPog.Core.Information;
using IdelPog.Core.Logging;
using IdelPog.Core.Logging.Writer;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Controller;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Core.Validation.Handler;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Inventory.Assertion;
using IdelPog.Inventory.Assertion.Interface;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Error;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Crafting.ECS;
using IdelPog.Inventory.Crafting.Factory;
using IdelPog.Inventory.Crafting.Factory.Interface;
using IdelPog.Inventory.Crafting.Mediator;
using IdelPog.Inventory.Factory;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Mediator;
using IdelPog.Inventory.Service;
using IdelPog.Inventory.Service.Interface;

namespace IdelPog.Inventory
{
    public static class InventoryBootstrapper
    {
        public static void RegisterFlows(IBufferManager bufferManager, IBatchRegister flowRegister)
        {
            IHandler throwHandler = new ThrowHandler();
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);
            IAmountAssertion amountAssertion = new AmountAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IItemFoundAssertion itemFoundAssertion = new ItemFoundAssertion(throwHandler);
            
            ILogWriter writer = new ConsoleWriter();
            IBufferLogger bufferLogger = new BufferLoggingService(writer);
            IMapper<ItemID> itemMapper = new Mapper<ItemID>(foundAssertion, uniqueAssertion);
            IItemFactory itemFactory = new ItemFactory(itemMapper);
            IItemInfoFactory itemInfoFactory = new ItemInfoFactory();
            
            IAssetRepository<RecipeID, CraftingRecipeEntity> recipeEntityRepository = new AssetRepository<RecipeID, CraftingRecipeEntity>();
            IStateRepository<ItemID, Item> itemRepository = new StateRepository<ItemID, Item>();
            IAssetRepository<ItemID,ItemDefinition> definitionRepository = new AssetRepository<ItemID, ItemDefinition>();
            
            IInventory inventory = new Service.Inventory(itemRepository, foundAssertion, uniqueAssertion, amountAssertion);
            IInventoryUpdateService inventoryUpdateService = new InventoryUpdateService(inventory, itemInfoFactory, itemFactory, collectionAssertion, itemFoundAssertion);
            
            RegisterInventoryUpdate(bufferManager, bufferLogger, flowRegister, inventoryUpdateService, itemMapper);
            RegisterRecipeCreation(bufferManager, bufferLogger, flowRegister, recipeEntityRepository);
            RegisterItemCraft(bufferManager, bufferLogger, flowRegister, recipeEntityRepository, inventory, inventoryUpdateService);
            RegisterItemDefinitionCreation(bufferManager, bufferLogger, flowRegister, definitionRepository);
        }

        /// <summary>
        /// Registers the <see cref="InventoryUpdate"/> flow
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="flowRegister">Used to register the InventoryUpdate flow</param>
        /// <param name="inventoryUpdateService">Handles updating the <see cref="IInventory"/></param>
        /// <param name="itemMapper">Maps <see cref="Information"/> to <see cref="ItemID"/>s</param>
        /// <remarks>
        /// Listens to -> <see cref="InventoryUpdate"/>. On Success -> <see cref="InventoryUpdateResponse"/>. On Error -> <see cref="InventoryUpdateError"/>
        /// </remarks>
        private static void RegisterInventoryUpdate(IBufferManager bufferManager, IBufferLogger bufferLogger, IBatchRegister flowRegister, IInventoryUpdateService inventoryUpdateService, IMapper<ItemID> itemMapper)
        {
            IHandler throwHandler = new ThrowHandler();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);

            // TODO: need some kinda ItemDefinition command that can define all this
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
            itemMapper.AddInformation(ItemID.RING, new Information { Description = "A small ring with some elvish writing on the inside", Name = "Ring" });
            
            IInventoryUpdateFactory updateFactory = new InventoryUpdateFactory();
            IInventoryUpdateSummarizer summarizer = new InventoryUpdateSummarizer(updateFactory, collectionAssertion);
            IDispatchMany<InventoryUpdateResponse> inventoryUpdateDispatcher = new ManagedDispatcher<InventoryUpdateResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            IBatchMediator<InventoryUpdate> inventoryMediator = new InventoryUpdateMediator(inventoryUpdateService, summarizer, inventoryUpdateDispatcher, collectionAssertion);

            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            IErrorFactory<InventoryUpdateError, IReadOnlyList<InventoryUpdate>> inventoryUpdateErrorFactory = new InventoryUpdateErrorFactory(baseErrorFactory);
            IBatchController<InventoryUpdate> inventoryController = new ManagedBatchController<InventoryUpdate>(inventoryMediator);
            
            flowRegister.RegisterBatch(inventoryController, inventoryUpdateErrorFactory);
        }

        /// <summary>
        /// Registers the <see cref="RecipeCreation"/> flow
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="flowRegister">Used to register the InventoryUpdate flow</param>
        /// <param name="recipeEntityRepository">Stores all <see cref="CraftingRecipeEntity"/></param>
        /// <remarks>
        /// Listens to -> <see cref="RecipeCreation"/>. On Success -> <see cref="RecipeCreationResponse"/>. On Error -> <see cref="RecipeCreationError"/>
        /// </remarks>
        private static void RegisterRecipeCreation(IBufferManager bufferManager, IBufferLogger bufferLogger, IBatchRegister flowRegister, IAssetRepository<RecipeID, CraftingRecipeEntity> recipeEntityRepository)
        {
            IHandler throwHandler = new ThrowHandler();
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);
            
            ICraftingRecipeEntityFactory entityFactory = new CraftingRecipeEntityFactory(throwHandler, collectionAssertion);
            IDispatchMany<RecipeCreationResponse> responseDispatcher = new ManagedDispatcher<RecipeCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            IBatchMediator<RecipeCreation> creationMediator = new RecipeCreationMediator(recipeEntityRepository, entityFactory, responseDispatcher, collectionAssertion, uniqueAssertion);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            RecipeCreationErrorFactory errorFactory = new(baseErrorFactory);
            IBatchController<RecipeCreation> creationController = new ManagedBatchController<RecipeCreation>(creationMediator);
            
            flowRegister.RegisterBatch(creationController, errorFactory);
        }

        /// <summary>
        /// Registers the <see cref="ItemCraft"/> flow
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="flowRegister">Used to register the InventoryUpdate flow</param>
        /// <param name="recipeEntityRepository">Stores all <see cref="CraftingRecipeEntity"/></param>
        /// <param name="inventory">Stores all <see cref="Item"/>s</param>
        /// <param name="inventoryUpdateService">Handles updating the <see cref="IInventory"/></param>
        /// <remarks>
        /// Listens to -> <see cref="ItemCraft"/>. On Success -> <see cref="ItemCraftResponse"/>. On Error -> <see cref="ItemCraftError"/>
        /// </remarks>
        private static void RegisterItemCraft(IBufferManager bufferManager, IBufferLogger bufferLogger, IBatchRegister flowRegister, IAssetRepository<RecipeID, CraftingRecipeEntity> recipeEntityRepository, IInventory inventory, IInventoryUpdateService inventoryUpdateService)
        {
            IHandler throwHandler = new ThrowHandler();
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            IAmountAssertion amountAssertion = new AmountAssertion(throwHandler);
            IFoundAssertion foundAssertion = new FoundAssertion(throwHandler);
            
            IInventoryUpdateFactory updateFactory = new InventoryUpdateFactory();
            IInventoryUpdateSummarizer inventoryUpdateSummarizer = new InventoryUpdateSummarizer(updateFactory, collectionAssertion);
            IDispatchMany<ItemCraftResponse> itemCraftDispatcher = new ManagedDispatcher<ItemCraftResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            IDispatchMany<InventoryUpdateResponse> inventoryUpdateDispatcher = new ManagedDispatcher<InventoryUpdateResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            IBatchMediator<ItemCraft> craftMediator = new ItemCraftMediator(inventory, recipeEntityRepository, inventoryUpdateService, updateFactory, inventoryUpdateSummarizer, itemCraftDispatcher, inventoryUpdateDispatcher, foundAssertion, amountAssertion, collectionAssertion);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            ItemCraftErrorFactory errorFactory = new(baseErrorFactory);
            IBatchController<ItemCraft> craftController = new ManagedBatchController<ItemCraft>(craftMediator);
            
            flowRegister.RegisterBatch(craftController, errorFactory);
        }

        /// <summary>
        /// Registers the <see cref="ItemDefinitionCreation"/> flow
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="flowRegister">Used to register the InventoryUpdate flow</param>
        /// <param name="definitionRepository">Stores all <see cref="ItemDefinition"/>s</param>
        /// <remarks>
        /// Listens to -> <see cref="ItemDefinitionCreation"/>. On Success -> <see cref="ItemDefinitionCreationResponse"/>. On Error -> <see cref="ItemDefinitionCreationError"/>
        /// </remarks>
        private static void RegisterItemDefinitionCreation(IBufferManager bufferManager, IBufferLogger bufferLogger, IBatchRegister flowRegister, IAssetRepository<ItemID, ItemDefinition> definitionRepository)
        {
            IHandler throwHandler = new ThrowHandler();
            ICollectionAssertion collectionAssertion = new CollectionAssertion(throwHandler);
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion(throwHandler);
            IUniqueAssertion uniqueAssertion = new UniqueAssertion(throwHandler);
            IAmountAssertion amountAssertion = new AmountAssertion(throwHandler);
            
            IDispatchMany<ItemDefinitionCreationResponse> responseDispatcher = new ManagedDispatcher<ItemDefinitionCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            IBatchMediator<ItemDefinitionCreation> creationMediator = new ItemDefinitionCreationMediator(definitionRepository, responseDispatcher, collectionAssertion, uniqueAssertion, amountAssertion);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            ItemDefinitionCreationErrorFactory errorFactory = new(baseErrorFactory);
            IBatchController<ItemDefinitionCreation> creationController = new ManagedBatchController<ItemDefinitionCreation>(creationMediator);
            
            flowRegister.RegisterBatch(creationController, errorFactory);
        }
    }
}
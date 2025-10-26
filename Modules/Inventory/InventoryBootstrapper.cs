using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Factory;
using IdelPog.Core.Factory.Interface;
using IdelPog.Core.Flows.Registry;
using IdelPog.Core.Logging;
using IdelPog.Core.Logging.Writer;
using IdelPog.Core.Messaging.Buffer.Manager;
using IdelPog.Core.Messaging.Controller;
using IdelPog.Core.Messaging.Dispatcher;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Repository.State;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Assertion.Interface;
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
            IFoundAssertion foundAssertion = new FoundAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            IAmountAssertion amountAssertion = new AmountAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IItemFoundAssertion itemFoundAssertion = new ItemFoundAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(foundAssertion, objectNullAssertion, uniqueAssertion);
            
            IAssetRepository<RecipeID, CraftingRecipeEntity> recipeEntityRepository = new AssetRepository<RecipeID, CraftingRecipeEntity>(repositoryAsserter);
            IStateRepository<ItemID, Item> itemRepository = new StateRepository<ItemID, Item>(repositoryAsserter);
            IAssetRepository<ItemID,ItemDefinition> definitionRepository = new AssetRepository<ItemID, ItemDefinition>(repositoryAsserter);
            
            ILogWriter writer = new ConsoleWriter();
            IBufferLogger bufferLogger = new BufferLoggingService(writer);
            IItemInfoFactory itemInfoFactory = new ItemInfoFactory();
            IItemCreationService creationService = new ItemCreationService(definitionRepository, foundAssertion);
            
            IInventory inventory = new Service.Inventory(itemRepository, foundAssertion, uniqueAssertion, amountAssertion);
            IInventoryUpdateService inventoryUpdateService = new InventoryUpdateService(inventory, itemInfoFactory, collectionAssertion, itemFoundAssertion, creationService);
            
            RegisterInventoryUpdate(bufferManager, bufferLogger, flowRegister, inventoryUpdateService);
            RegisterRecipeCreation(bufferManager, bufferLogger, flowRegister, recipeEntityRepository, repositoryAsserter);
            RegisterItemCraft(bufferManager, bufferLogger, flowRegister, recipeEntityRepository, inventory, inventoryUpdateService);
            RegisterItemDefinitionCreation(bufferManager, bufferLogger, flowRegister, definitionRepository);
            RegisterItemSell(bufferManager, bufferLogger, flowRegister, inventoryUpdateService, definitionRepository);
        }

        /// <summary>
        /// Registers the <see cref="InventoryUpdate"/> flow
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="flowRegister">Used to register the InventoryUpdate flow</param>
        /// <param name="inventoryUpdateService">Handles updating the <see cref="IInventory"/></param>
        /// <remarks>
        /// Listens to -> <see cref="InventoryUpdate"/>. On Success -> <see cref="InventoryUpdateResponse"/>. On Error -> <see cref="InventoryUpdateError"/>
        /// </remarks>
        private static void RegisterInventoryUpdate(IBufferManager bufferManager, IBufferLogger bufferLogger, IBatchRegister flowRegister, IInventoryUpdateService inventoryUpdateService)
        {
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            
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
        /// <param name="repositoryAsserter">Asserts the Repository</param>
        /// <remarks>
        /// Listens to -> <see cref="RecipeCreation"/>. On Success -> <see cref="RecipeCreationResponse"/>. On Error -> <see cref="RecipeCreationError"/>
        /// </remarks>
        private static void RegisterRecipeCreation(IBufferManager bufferManager, IBufferLogger bufferLogger, IBatchRegister flowRegister, IAssetRepository<RecipeID, CraftingRecipeEntity> recipeEntityRepository, IRepositoryAsserter repositoryAsserter)
        {
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            
            ICraftingRecipeEntityFactory entityFactory = new CraftingRecipeEntityFactory(collectionAssertion, repositoryAsserter);
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
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IAmountAssertion amountAssertion = new AmountAssertion();
            IFoundAssertion foundAssertion = new FoundAssertion();
            
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
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IUniqueAssertion uniqueAssertion = new UniqueAssertion();
            IAmountAssertion amountAssertion = new AmountAssertion();
            
            IDispatchMany<ItemDefinitionCreationResponse> responseDispatcher = new ManagedDispatcher<ItemDefinitionCreationResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            
            IBatchMediator<ItemDefinitionCreation> creationMediator = new ItemDefinitionCreationMediator(definitionRepository, responseDispatcher, collectionAssertion, uniqueAssertion, amountAssertion);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            ItemDefinitionCreationErrorFactory errorFactory = new(baseErrorFactory);
            IBatchController<ItemDefinitionCreation> creationController = new ManagedBatchController<ItemDefinitionCreation>(creationMediator);
            
            flowRegister.RegisterBatch(creationController, errorFactory);
        }

        /// <summary>
        /// Registers the <see cref="ItemSell"/> flow
        /// </summary>
        /// <param name="bufferManager">Used to dispatch response records</param>
        /// <param name="bufferLogger">Logs all messages in and out</param>
        /// <param name="flowRegister">Used to register the InventoryUpdate flow</param>
        /// <param name="inventoryUpdateService">Handles updating the <see cref="IInventory"/></param>
        /// <param name="definitionRepository">Stores all <see cref="ItemDefinition"/>s</param>
        /// <remarks>
        /// Listens to -> <see cref="ItemSell"/>. On Success -> <see cref="ItemSellResponse"/>. On Error -> <see cref="ItemSellError"/>
        /// </remarks>
        private static void RegisterItemSell(IBufferManager bufferManager, IBufferLogger bufferLogger, IBatchRegister flowRegister, IInventoryUpdateService inventoryUpdateService, IAssetRepository<ItemID, ItemDefinition> definitionRepository)
        {
            ICollectionAssertion collectionAssertion = new CollectionAssertion();
            IObjectNullAssertion objectNullAssertion = new ObjectNullAssertion();
            IAmountAssertion amountAssertion = new AmountAssertion();
            IFoundAssertion foundAssertion = new FoundAssertion();
            
            IInventoryUpdateFactory updateFactory = new InventoryUpdateFactory();
            
            IInventoryUpdateSummarizer inventoryUpdateSummarizer = new InventoryUpdateSummarizer(updateFactory, collectionAssertion);
            IDispatchMany<ItemSellResponse> itemSellDispatcher = new ManagedDispatcher<ItemSellResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            IDispatchMany<InventoryUpdateResponse> inventoryUpdateDispatcher = new  ManagedDispatcher<InventoryUpdateResponse>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
            IDispatchMany<CurrencyUpdate> currencyUpdateDispatcher =  new ManagedDispatcher<CurrencyUpdate>(bufferManager, bufferLogger, objectNullAssertion, collectionAssertion);
                
            IBatchMediator<ItemSell> sellMediator = new ItemSellMediator(definitionRepository, inventoryUpdateService, inventoryUpdateSummarizer, updateFactory, itemSellDispatcher, inventoryUpdateDispatcher, currencyUpdateDispatcher, collectionAssertion, amountAssertion, foundAssertion);
            
            IBaseErrorFactory baseErrorFactory = new BaseErrorFactory();
            ItemSellErrorFactory errorFactory = new(baseErrorFactory);
            IBatchController<ItemSell> sellController = new ManagedBatchController<ItemSell>(sellMediator);
            
            flowRegister.RegisterBatch(sellController, errorFactory);
        }
    }
}
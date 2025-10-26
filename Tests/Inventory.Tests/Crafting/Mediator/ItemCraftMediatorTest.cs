using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Repository.Asserter;
using IdelPog.Core.Repository.Asset;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Crafting.ECS;
using IdelPog.Inventory.Crafting.ECS.Component;
using IdelPog.Inventory.Crafting.Mediator;
using IdelPog.Inventory.Exceptions;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Service.Interface;
using Moq;

namespace IdelPog.Inventory.Tests.Crafting.Mediator
{
    [TestFixture]
    public sealed class ItemCraftMediatorTest
    {
        private IBatchMediator<ItemCraft> _craftMediator;
        private Mock<IInventory> _inventoryMock;
        private Mock<IAssetRepository<RecipeID, CraftingRecipeEntity>> _entityRepositoryMock;
        private Mock<IInventoryUpdateService> _updateServiceMock;
        private Mock<IDispatchMany<ItemCraftResponse>> _dispatcherMock;
        private Mock<IInventoryUpdateFactory> _updateFactoryMock;
        private Mock<IDispatchMany<InventoryUpdateResponse>> _updateDispatcherMock;
        private Mock<IInventoryUpdateSummarizer> _updateSummarizerMock;
        
        private ItemCraft _ironRingCraft;
        private CraftingRecipeEntity _recipeEntity;
        private Item _ironRingItem;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _inventoryMock = new Mock<IInventory>();
            _entityRepositoryMock = new Mock<IAssetRepository<RecipeID, CraftingRecipeEntity>>();
            _updateServiceMock = new Mock<IInventoryUpdateService>();
            _dispatcherMock = new Mock<IDispatchMany<ItemCraftResponse>>();
            _updateFactoryMock = new Mock<IInventoryUpdateFactory>();
            _updateDispatcherMock = new Mock<IDispatchMany<InventoryUpdateResponse>>();
            _updateSummarizerMock = new Mock<IInventoryUpdateSummarizer>();
            IRepositoryAsserter repositoryAsserter = new RepositoryAsserter(new FoundAssertion(), new ObjectNullAssertion(), new UniqueAssertion());
            
            _ironRingItem = new Item(ItemID.RING, 1, new Information { Name = "", Description = "" }, 1);
            _ironRingCraft = new ItemCraft { RecipeID = RecipeID.IRON_RING, Amount = 1 };
            _recipeEntity = new CraftingRecipeEntity(repositoryAsserter, [new RecipeInputComponent { ItemID = ItemID.IRON, RequiredAmount = 1}], [new RecipeOutputComponent { ItemID = ItemID.RING, OutputAmount = 1}]);
            _craftMediator = new ItemCraftMediator(_inventoryMock.Object, _entityRepositoryMock.Object, _updateServiceMock.Object, _updateFactoryMock.Object, _updateSummarizerMock.Object, _dispatcherMock.Object, _updateDispatcherMock.Object, new FoundAssertion(), new AmountAssertion(), new CollectionAssertion());
        }

        [SetUp]
        public void Setup()
        {
            _entityRepositoryMock.Reset();
            _updateServiceMock.Reset();
            _dispatcherMock.Reset();
        }

        private void SetupInventoryContains(bool contains)
        {
            _inventoryMock.Setup(library => library.Contains(It.IsAny<ItemID>())).Returns(contains);
        }

        private void SetupInventoryGet(Item item)
        {
            _inventoryMock.Setup(library => library.GetItem(It.IsAny<ItemID>())).Returns(item);
        }

        private void SetupRepository(RecipeID recipeID, bool contains)
        {
            _entityRepositoryMock.Setup(library => library.Contains(recipeID)).Returns(contains);
            _entityRepositoryMock.Setup(library => library.Get(recipeID)).Returns(_recipeEntity);
        }
        
        private void VerifyRepository(Times times)
        {
            _entityRepositoryMock.Verify(library => library.Contains(It.IsAny<RecipeID>()), times);
            _entityRepositoryMock.Verify(library => library.Get(It.IsAny<RecipeID>()), times);
            VerifyRepositoryNoOtherCalls();
        }
        
        private void VerifyRepositoryNoOtherCalls()
        {
            _entityRepositoryMock.VerifyNoOtherCalls();
        }

        private void VerifyDispatcherCalled(Times times)
        {
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<ItemCraftResponse[]>()), times);
        }

        private void VerifyUpdateServiceApplyUpdatesCalled(Times times)
        { 
            _updateServiceMock.Verify(library => library.ApplyUpdates(It.IsAny<IReadOnlyList<InventoryUpdate>>()), times);
        }
        
        private void VerifyServiceNoOtherCalls()
        {
            _updateServiceMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_CraftsItem_DispatchesResponse()
        {
            SetupRepository(_ironRingCraft.RecipeID, true);
            SetupInventoryContains(true);
            SetupInventoryGet(_ironRingItem);
            
            Assert.DoesNotThrow(() => _craftMediator.HandleMessages([_ironRingCraft]));

            VerifyRepository(Times.Once());
            VerifyDispatcherCalled(Times.Once());
            VerifyUpdateServiceApplyUpdatesCalled(Times.Once());
            VerifyServiceNoOtherCalls();
        }

        [Test]
        public void Positive_HandleMessages_MultipleMessages_DispatchesResponse()
        {
            SetupRepository(_ironRingCraft.RecipeID, true);
            SetupInventoryContains(true);
            SetupInventoryGet(_ironRingItem);
            
            Assert.DoesNotThrow(() => _craftMediator.HandleMessages([_ironRingCraft, _ironRingCraft]));

            VerifyDispatcherCalled(Times.Once());
            VerifyUpdateServiceApplyUpdatesCalled(Times.Once());
            VerifyServiceNoOtherCalls();
        }

        [Test]
        public void Negative_HandleMessages_EmptyMessages_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _craftMediator.HandleMessages([]));
            
            VerifyServiceNoOtherCalls();
            VerifyRepositoryNoOtherCalls();
            VerifyDispatcherCalled(Times.Never());
        }
        
        [Test]
        public void Negative_HandleMessages_NullMessages_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _craftMediator.HandleMessages(null!));
            
            VerifyServiceNoOtherCalls();
            VerifyRepositoryNoOtherCalls();
            VerifyDispatcherCalled(Times.Never());
        }

        [Test]
        public void Negative_HandleMessages_EntityNotFound_Throws()
        {
            Assert.Throws<NotFoundException<RecipeID>>(() => _craftMediator.HandleMessages([_ironRingCraft]));
            
            VerifyServiceNoOtherCalls();
            VerifyDispatcherCalled(Times.Never());
            
            _entityRepositoryMock.Verify(library => library.Contains(_ironRingCraft.RecipeID), Times.Once);
            VerifyRepositoryNoOtherCalls();
        }
        
        [Test]
        public void Negative_HandleMessages_ItemNotFound_Throws()
        {
            SetupRepository(_ironRingCraft.RecipeID, true);
            SetupInventoryContains(false);
            
            Assert.Throws<NotFoundException<ItemID>>(() => _craftMediator.HandleMessages([_ironRingCraft]));
            
            VerifyServiceNoOtherCalls();
            VerifyDispatcherCalled(Times.Never());
            VerifyRepository(Times.Once());
        }

        [Test]
        public void Negative_HandleMessages_ItemNotEnoughAmount_Throws()
        {
            Item itemClone = _ironRingItem.DeepClone();
            itemClone.Amount = 0;
            
            SetupRepository(_ironRingCraft.RecipeID, true);
            SetupInventoryContains(true);
            SetupInventoryGet(itemClone);
            
            Assert.Throws<InsufficientAmountException>(() => _craftMediator.HandleMessages([_ironRingCraft]));
            
            VerifyServiceNoOtherCalls();
            VerifyDispatcherCalled(Times.Never());
            VerifyRepository(Times.Once());
        }

        [Test]
        public void Negative_HandleMessages_CraftAmountZero_Throws()
        {
            SetupRepository(_ironRingCraft.RecipeID, true);
            
            Assert.Throws<AmountZeroException>(() => _craftMediator.HandleMessages([_ironRingCraft with {  Amount = 0 }]));
            
            VerifyServiceNoOtherCalls();
            VerifyDispatcherCalled(Times.Never());
            
            _entityRepositoryMock.Verify(library => library.Contains(_ironRingCraft.RecipeID), Times.Once);
            VerifyRepositoryNoOtherCalls();
        }
    }
}
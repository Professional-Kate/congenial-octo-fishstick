using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Dispatcher.Buffer;
using IdelPog.Core.Messaging.Listener.Buffer;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Mediator;
using IdelPog.Inventory.Service.Interface;
using Moq;

namespace IdelPog.Inventory.Tests
{
    [TestFixture]
    public sealed class InventoryUpdateMediatorTest
    {
        private IBatchMediator<InventoryUpdate> _inventoryMediator;
        private Mock<IInventoryUpdateService> _inventoryUpdateServiceMock;
        private Mock<IInventoryUpdateSummarizer> _updateSummarizerMock;
        private Mock<IDispatchMany<InventoryUpdateResponse>> _dispatcherMock;

        private InventoryUpdate _addStoneUpdate;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _inventoryUpdateServiceMock = new Mock<IInventoryUpdateService>();
            _updateSummarizerMock = new Mock<IInventoryUpdateSummarizer>();
            _dispatcherMock = new Mock<IDispatchMany<InventoryUpdateResponse>>();

            _inventoryMediator = new InventoryUpdateMediator(_inventoryUpdateServiceMock.Object, _updateSummarizerMock.Object, _dispatcherMock.Object, new CollectionAssertion(new ThrowHandler()));

            _addStoneUpdate = new InventoryUpdate
            {
                ActionType = ActionType.ADD,
                Amount = 1,
                ItemID = ItemID.STONE
            };
        }

        [SetUp]
        public void Setup()
        {
            _dispatcherMock.Reset();
            _updateSummarizerMock.Reset();
            _inventoryUpdateServiceMock.Reset();
        }

        private void SetupSummarizerMock(InventoryUpdate[] inputUpdates, InventoryUpdate[] summaryUpdates)
        {
            _updateSummarizerMock.Setup(library => library.GetSummary(inputUpdates)).Returns(summaryUpdates);
        }

        private void VerifyDispatcherCalled()
        {
            _dispatcherMock.Verify(library => library.Dispatch(It.IsAny<InventoryUpdateResponse[]>()), Times.Once);
        }

        private void VerifyUpdateServiceCalled()
        {
            _inventoryUpdateServiceMock.Verify(library => library.ApplyUpdates(It.IsAny<IReadOnlyList<InventoryUpdate>>()), Times.Once);
            
        }

        [Test]
        public void Positive_HandleMessages_SingleMessage_CreateItem()
        {
            SetupSummarizerMock([_addStoneUpdate], [_addStoneUpdate]);
            
            Assert.DoesNotThrow(() => _inventoryMediator.HandleMessages([_addStoneUpdate]));

            VerifyUpdateServiceCalled();
            VerifyDispatcherCalled();
        }

        [Test]
        public void Positive_HandleMessages_SingleMessage_AddsToItem_NoCreation()
        {
            SetupSummarizerMock([_addStoneUpdate], [_addStoneUpdate]);
            
            Assert.DoesNotThrow(() => _inventoryMediator.HandleMessages([_addStoneUpdate]));
            
            VerifyUpdateServiceCalled();
            VerifyDispatcherCalled();
        }

        [Test]
        public void Positive_HandleMessages_MultipleMessages_SingleItemID_DispatchesOneCombinedUpdate()
        {
            SetupSummarizerMock([_addStoneUpdate, _addStoneUpdate, _addStoneUpdate], [_addStoneUpdate with { Amount = 3 }]);
            
            Assert.DoesNotThrow(() => _inventoryMediator.HandleMessages([_addStoneUpdate, _addStoneUpdate, _addStoneUpdate]));
            
            VerifyUpdateServiceCalled();
            VerifyDispatcherCalled();
        }

        [Test]
        public void Positive_HandleMessages_SingleMessage_RemoveItem()
        {
            InventoryUpdate removeStoneUpdate = _addStoneUpdate with { ActionType = ActionType.REMOVE };
            SetupSummarizerMock([removeStoneUpdate], [removeStoneUpdate]);
            
            Assert.DoesNotThrow(() => _inventoryMediator.HandleMessages([removeStoneUpdate]));
            
            VerifyUpdateServiceCalled();
            VerifyDispatcherCalled();
        }

        [Test]
        public void Positive_HandleMessages_MultipleItemIDs_InvokesTwice_DispatchesOnce()
        {
            InventoryUpdate addGoldUpdate = _addStoneUpdate with { ItemID = ItemID.GOLD };
            SetupSummarizerMock([_addStoneUpdate, addGoldUpdate], [_addStoneUpdate, addGoldUpdate]);
            
            Assert.DoesNotThrow(() => _inventoryMediator.HandleMessages([_addStoneUpdate, addGoldUpdate]));
            
            VerifyDispatcherCalled();
            VerifyUpdateServiceCalled();
        }

        [Test]
        public void Negative_HandleMessages_EmptyMessages_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _inventoryMediator.HandleMessages([]));
        }

        [Test]
        public void Negative_HandleMessages_NullMessages_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _inventoryMediator.HandleMessages(null!));
        }

        [Test]
        public void Negative_HandleMessage_SummarizerReturnsNothing_Throws()
        {
            InventoryUpdate removeStoneUpdate = _addStoneUpdate with { ActionType = ActionType.REMOVE };
            SetupSummarizerMock([_addStoneUpdate, removeStoneUpdate], []);
            
            Assert.Throws<EmptyCollectionException>(() => _inventoryMediator.HandleMessages([removeStoneUpdate, _addStoneUpdate]));
        }
    }
}
using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Inventory.Assertion;
using IdelPog.Inventory.Contracts;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Service;
using IdelPog.Inventory.Service.Interface;
using Moq;

namespace IdelPog.Inventory.Tests.Service
{
    [TestFixture]
    public sealed class InventoryUpdateServiceTest
    {
        private InventoryUpdateService _updateService;
        private Mock<IInventory> _inventoryMock;
        private Mock<IItemInfoFactory> _infoFactoryMock;
        private Mock<IItemFactory> _itemFactoryMock;

        private Item _spruceItem;
        private InventoryUpdate _spruceUpdate;
        private ItemInfo _spruceInfo;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _spruceItem = new Item(ItemID.SPRUCE, 1, new Information { Name = "", Description = "" }, 1);
            _spruceUpdate = new InventoryUpdate { ItemID = ItemID.SPRUCE, Amount = 1, ActionType = ActionType.ADD };
            _spruceInfo = new ItemInfo { ItemID = ItemID.SPRUCE, BaseSellPrice = 1, Amount = 1, Information = new Information { Description = "", Name = "" }};
            
            _inventoryMock = new Mock<IInventory>();
            _infoFactoryMock = new Mock<IItemInfoFactory>();
            _itemFactoryMock = new Mock<IItemFactory>();
            ThrowHandler throwHandler = new();
            
            _updateService = new InventoryUpdateService(_inventoryMock.Object, _infoFactoryMock.Object, _itemFactoryMock.Object, new CollectionAssertion(throwHandler), new ItemFoundAssertion(throwHandler));
        }

        [SetUp]
        public void Setup()
        {
            _inventoryMock.Reset();
            _infoFactoryMock.Reset();
            _itemFactoryMock.Reset();
        }

        private void SetupInventoryContains(ItemID itemID, bool contains)
        {
            _inventoryMock.Setup(library => library.Contains(itemID)).Returns(contains);
        }

        private void SetupInventoryGet(Item item)
        {
            _inventoryMock.Setup(library => library.GetItem(item.ItemID)).Returns(item);
        }

        private void SetupInventoryRemoveAmount(ItemID itemID, uint amount, MutateType mutateType)
        {
            _inventoryMock.Setup(library => library.RemoveAmount(itemID, amount)).Returns(mutateType);
        }

        private void SetupInfoFactory(ItemInfo itemInfo)
        {
            _infoFactoryMock.Setup(library => library.Create(itemInfo.ItemID, itemInfo.BaseSellPrice, itemInfo.Amount, itemInfo.Information)).Returns(itemInfo);
        }

        private void SetupItemFactory(Item item)
        {
            _itemFactoryMock.Setup(library => library.CreateItem(item.ItemID, item.Amount)).Returns(item);
        }

        private static void VerifyResponseLength(int length, IReadOnlyList<InventoryUpdateResponse> responses)
        {
            Assert.That(responses, Has.Count.EqualTo(length));
        }

        private static void VerifyResponse(InventoryUpdateResponse response, ItemInfo itemInfo, MutateType mutateType)
        {
            Assert.Multiple(() =>
            {
                Assert.That(response.MutateType, Is.EqualTo(mutateType));
                Assert.That(response.ItemInfo, Is.EqualTo(itemInfo));
            });
        }

        private void VerifyInventoryContains(Times times, ItemID itemID)
        {
            _inventoryMock.Verify(library => library.Contains(itemID), times);
        }

        private void VerifyInventoryAdd(Times times, Item item)
        {
            _inventoryMock.Verify(library => library.AddItem(item), times);
        }

        private void VerifyInventoryGet(Times times, Item item)
        {
            _inventoryMock.Verify(library => library.GetItem(item.ItemID), times);
        }

        private void VerifyInventoryAddAmount(Times times, ItemID itemID, uint amount)
        {
            _inventoryMock.Verify(library => library.AddAmount(itemID, amount), times);
        }
        
        private void VerifyInventoryRemoveAmount(Times times, ItemID itemID, uint amount)
        {
            _inventoryMock.Verify(library => library.RemoveAmount(itemID, amount), times);
        }

        private void VerifyInventoryNoOtherCalls()
        {
            _inventoryMock.VerifyNoOtherCalls();
        }

        [Test]
        public void Positive_ApplyUpdates_SingleAddMessage_CreatesItem_ReturnsResponse()
        {
            SetupInventoryContains(_spruceUpdate.ItemID, false);
            SetupInfoFactory(_spruceInfo);
            SetupItemFactory(_spruceItem);
            
            IReadOnlyList<InventoryUpdateResponse> responses = _updateService.ApplyUpdates([_spruceUpdate]);
            
            VerifyResponseLength(1, responses);
            VerifyResponse(responses[0], _spruceInfo, MutateType.CREATED);
            VerifyInventoryContains(Times.Once(), _spruceUpdate.ItemID);
            VerifyInventoryAdd(Times.Once(), _spruceItem);
            VerifyInventoryNoOtherCalls();
        }

        [Test]
        public void Positive_ApplyUpdates_SingleAddMessage_AddsToItem_ReturnsResponse()
        {
            SetupInventoryContains(_spruceUpdate.ItemID, true);
            SetupInventoryGet(_spruceItem);
            SetupInfoFactory(_spruceInfo);
            SetupItemFactory(_spruceItem);
            
            IReadOnlyList<InventoryUpdateResponse> responses = _updateService.ApplyUpdates([_spruceUpdate]);
            
            VerifyResponseLength(1, responses);
            VerifyResponse(responses[0], _spruceInfo, MutateType.CHANGED);
            VerifyInventoryContains(Times.Once(), _spruceUpdate.ItemID);
            VerifyInventoryGet(Times.Once(), _spruceItem);
            VerifyInventoryAddAmount(Times.Once(), _spruceUpdate.ItemID, 1);
            VerifyInventoryNoOtherCalls();
        }

        [Test]
        public void Positive_ApplyUpdates_SingleRemoveMessage_RemovesItem_ReturnsResponse()
        {
            SetupInventoryContains(_spruceUpdate.ItemID, true);
            SetupInventoryGet(_spruceItem);
            SetupInventoryRemoveAmount(_spruceUpdate.ItemID, _spruceUpdate.Amount, MutateType.DELETED);
            SetupInfoFactory(_spruceInfo with { Amount = 0 });
            SetupItemFactory(_spruceItem);
            
            IReadOnlyList<InventoryUpdateResponse> responses = _updateService.ApplyUpdates([_spruceUpdate with { ActionType = ActionType.REMOVE }]);
            
            VerifyResponseLength(1, responses);
            VerifyResponse(responses[0], _spruceInfo with { Amount = 0 }, MutateType.DELETED);
            VerifyInventoryContains(Times.Once(), _spruceUpdate.ItemID);
            VerifyInventoryGet(Times.Once(), _spruceItem);
            VerifyInventoryRemoveAmount(Times.Once(), _spruceUpdate.ItemID, 1);
            VerifyInventoryNoOtherCalls();
        }

        [Test]
        public void Positive_ApplyUpdates_SingleRemoveMessage_RemovesFromItem_ReturnsResponse()
        {
            SetupInventoryContains(_spruceUpdate.ItemID, true);
            SetupInventoryGet(_spruceItem);
            SetupInventoryRemoveAmount(_spruceUpdate.ItemID, _spruceUpdate.Amount, MutateType.CHANGED);
            SetupInfoFactory(_spruceInfo with { Amount = 1 });
            SetupItemFactory(_spruceItem);
            
            IReadOnlyList<InventoryUpdateResponse> responses = _updateService.ApplyUpdates([_spruceUpdate with { ActionType = ActionType.REMOVE }]);
            
            VerifyResponseLength(1, responses);
            VerifyResponse(responses[0], _spruceInfo with { Amount = 1 }, MutateType.CHANGED);
            VerifyInventoryContains(Times.Once(), _spruceUpdate.ItemID);
            VerifyInventoryGet(Times.Exactly(2), _spruceItem);
            VerifyInventoryRemoveAmount(Times.Once(), _spruceUpdate.ItemID, 1);
            VerifyInventoryNoOtherCalls();
        }

        [Test]
        public void Positive_ApplyUpdates_MultipleUpdates_ReturnsResponses()
        {
            SetupInventoryContains(_spruceUpdate.ItemID, true);
            SetupInventoryGet(_spruceItem);
            SetupInfoFactory(_spruceInfo);
            SetupItemFactory(_spruceItem);
            
            IReadOnlyList<InventoryUpdateResponse> responses = _updateService.ApplyUpdates([_spruceUpdate, _spruceUpdate]);
            
            VerifyResponseLength(2, responses);
            VerifyResponse(responses[0], _spruceInfo, MutateType.CHANGED);
            VerifyInventoryContains(Times.Exactly(2), _spruceUpdate.ItemID);
            VerifyInventoryGet(Times.Exactly(2), _spruceItem);
            VerifyInventoryAddAmount(Times.Exactly(2), _spruceUpdate.ItemID, 1);
            VerifyInventoryNoOtherCalls();
        }

        [Test]
        public void Negative_ApplyUpdates_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _updateService.ApplyUpdates([]));
            
            VerifyInventoryNoOtherCalls();
        }
        
        [Test]
        public void Negative_ApplyUpdates_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _updateService.ApplyUpdates(null!));
            
            VerifyInventoryNoOtherCalls();
        }

        [Test]
        public void Negative_ApplyUpdates_RemoveUpdate_ItemNotFound_Throws()
        { 
            SetupInventoryContains(_spruceUpdate.ItemID, false);
            
            Assert.Throws<NotFoundException<ItemID>>(() => _updateService.ApplyUpdates([_spruceUpdate with { ActionType = ActionType.REMOVE }]));
            
            VerifyInventoryContains(Times.Once(), _spruceUpdate.ItemID);
            VerifyInventoryNoOtherCalls();
        }
    }
}
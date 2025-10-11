using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Inventory.Contracts.Error;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Exceptions;

namespace IdelPog.Integration.Tests.Inventory
{
    [TestFixture]
    public sealed class InventoryUpdateTest : ManagedTestBuffer
    {
        private InventoryUpdateResponseListener _inventoryUpdateResponseListener;
        private InventoryUpdateErrorListener _inventoryUpdateErrorListener;

        private InventoryUpdate _addStoneUpdate;
        private ItemInfo _stoneInfo;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _addStoneUpdate = new InventoryUpdate
            {
                Amount = 10,
                ActionType = ActionType.ADD,
                ItemID = ItemID.STONE
            };

            _stoneInfo = new ItemInfo
            {
                Amount = 10,
                ItemID = ItemID.STONE,
                BaseSellPrice = 1,
                Information = new Information { Description = "", Name = "" }
            };
        }
        
        [SetUp]
        public void Setup()
        {
            _inventoryUpdateResponseListener = new InventoryUpdateResponseListener();
            _inventoryUpdateErrorListener = new InventoryUpdateErrorListener();
            
            ManagedSubscribe(_inventoryUpdateResponseListener);
            ManagedSubscribe(_inventoryUpdateErrorListener);
        }

        private void DispatchInventoryUpdate(params InventoryUpdate[] inventoryUpdates)
        {
            IBuffer<InventoryUpdate> buffer = BufferManager.RequestBuffer<InventoryUpdate>(new BufferRequest(inventoryUpdates.Length));
            buffer.Assign(inventoryUpdates);
            buffer.MarkReady();
        }

        private void AssertResponseListenerCalled(bool wasCalled)
        { 
            Assert.That(_inventoryUpdateResponseListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertResponseLength(int length)
        {
            Assert.That(_inventoryUpdateResponseListener.InventoryUpdateResponses, Has.Length.EqualTo(length));
        }
        
        private static void AssertResponse(InventoryUpdateResponse response, ItemInfo expectedItemInfo, MutateType expectedMutateType)
        {
            Assert.That(response.MutateType, Is.EqualTo(expectedMutateType));
            
            ItemInfo itemInfo = response.ItemInfo;
            Assert.Multiple(() =>
            {
                Assert.That(itemInfo.ItemID, Is.EqualTo(expectedItemInfo.ItemID));
                Assert.That(itemInfo.Amount, Is.EqualTo(expectedItemInfo.Amount));
                Assert.That(itemInfo.BaseSellPrice, Is.EqualTo(expectedItemInfo.BaseSellPrice));
            });
        }
        
        private void AssertErrorListenerCalled(bool wasCalled)
        { 
            Assert.That(_inventoryUpdateErrorListener.WasCalled, Is.EqualTo(wasCalled));
        }

        private void AssertErrorLength(int length)
        {
            Assert.That(_inventoryUpdateErrorListener.InventoryUpdateError.InventoryUpdates, Has.Length.EqualTo(length));
        }

        private void AssertResponseError<TException>()
        {
            InventoryUpdateError updateError = _inventoryUpdateErrorListener.InventoryUpdateError;
            BaseError baseError = updateError.BaseError;

            Assert.Multiple(() =>
            {
                Assert.That(baseError.Exception.InnerException, Is.TypeOf<TException>());
                Assert.That(baseError.Exception.Message, Is.Not.Null.And.Not.Empty);
            });
        }

        [Test]
        public void Positive_SendAddUpdate_CreatesStone_DispatchesCreationUpdate()
        {
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_inventoryUpdateResponseListener.InventoryUpdateResponses[0], _stoneInfo, MutateType.CREATED);
        }

        [Test]
        public void Positive_SendMultipleAddUpdates_CreatesMultipleItems_DispatchesCreationUpdate()
        {
            InventoryUpdate addCopper = _addStoneUpdate with { ItemID = ItemID.COPPER };
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate, addCopper));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_inventoryUpdateResponseListener.InventoryUpdateResponses[0], _stoneInfo, MutateType.CREATED);
            AssertResponse(_inventoryUpdateResponseListener.InventoryUpdateResponses[1], _stoneInfo with { ItemID = ItemID.COPPER }, MutateType.CREATED);
            
        }

        [Test]
        public void Positive_SendAddAfterCreation_ResponsesWithChanged()
        {
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));

            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_inventoryUpdateResponseListener.InventoryUpdateResponses[0], _stoneInfo with { Amount = 20 }, MutateType.CHANGED);
        }

        [Test]
        public void Positive_SendRemoveAfterCreation_ResponsesWithDeleted()
        {
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE }));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_inventoryUpdateResponseListener.InventoryUpdateResponses[0], _stoneInfo with { Amount = 0, BaseSellPrice = 0 }, MutateType.DELETED);
        }

        [Test]
        public void Positive_SendMixedUpdate_SingleItem_ResponsesWithCorrectAmount()
        { 
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE }, _addStoneUpdate, _addStoneUpdate));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_inventoryUpdateResponseListener.InventoryUpdateResponses[0], _stoneInfo, MutateType.CREATED);
        }

        [Test]
        public void Positive_SendMixedUpdates_TwoItems_OneZero_DispatchesOneResponse()
        {
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE }, _addStoneUpdate, _addStoneUpdate with { ItemID = ItemID.COPPER }));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_inventoryUpdateResponseListener.InventoryUpdateResponses[0], _stoneInfo with { ItemID = ItemID.COPPER }, MutateType.CREATED);
        }

        [Test]
        public void Negative_SendMixedUpdates_OneItem_AmountZero_DispatchesError()
        {
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE }, _addStoneUpdate));
            
            Assert.Multiple(() =>
            {
                Assert.That(_inventoryUpdateErrorListener.WasCalled, Is.True);
                Assert.That(_inventoryUpdateResponseListener.WasCalled, Is.False);
            });

            AssertResponseError<EmptyCollectionException>();

            InventoryUpdate[] inventoryUpdates = _inventoryUpdateErrorListener.InventoryUpdateError.InventoryUpdates;
            Assert.That(inventoryUpdates, Has.Length.EqualTo(2));
        }

        [Test]
        public void Negative_SendRemoveUpdate_ItemNotCreated_DispatchesError()
        {
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE }));
            
            Assert.Multiple(() =>
            {
                Assert.That(_inventoryUpdateErrorListener.WasCalled, Is.True);
                Assert.That(_inventoryUpdateResponseListener.WasCalled, Is.False);
            });

            AssertResponseError<NotFoundException<ItemID>>();

            InventoryUpdate[] inventoryUpdates = _inventoryUpdateErrorListener.InventoryUpdateError.InventoryUpdates;
            Assert.That(inventoryUpdates, Has.Length.EqualTo(1));
        }

        [Test]
        public void Negative_SendMixedUpdates_FinalAmountNegative_DispatchesError()
        {
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE, Amount = 11 }));
            
            Assert.That(_inventoryUpdateErrorListener.WasCalled, Is.True);
            AssertResponseError<InsufficientAmountException>();

            InventoryUpdate[] inventoryUpdates = _inventoryUpdateErrorListener.InventoryUpdateError.InventoryUpdates;
            Assert.That(inventoryUpdates, Has.Length.EqualTo(1));
        }

        [Test]
        public void Negative_SendBadActionType_DispatchesError()
        {
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = (ActionType) 5 }));
            
            Assert.That(_inventoryUpdateErrorListener.WasCalled, Is.True);
            AssertResponseError<ArgumentOutOfRangeException>();

            InventoryUpdate[] inventoryUpdates = _inventoryUpdateErrorListener.InventoryUpdateError.InventoryUpdates;
            Assert.That(inventoryUpdates, Has.Length.EqualTo(1));
        }
    }
}
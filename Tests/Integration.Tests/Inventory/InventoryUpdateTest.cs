using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Inventory.Exceptions;

namespace IdelPog.Integration.Tests.Inventory
{
    [TestFixture]
    public class InventoryUpdateTest : ManagedTestBuffer
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
                BaseSellPrice = 1
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

        private void AssertResponseEntry(InventoryUpdateEntry entry, InventoryUpdate expectedUpdate, ItemInfo expectedItemInfo)
        {
            InventoryUpdate inventoryUpdate = entry.InventoryUpdate;
            ItemInfo itemInfo = entry.ItemInfo;
            
            Assert.Multiple(() =>
            {
                Assert.That(inventoryUpdate.ActionType, Is.EqualTo(expectedUpdate.ActionType));
                Assert.That(inventoryUpdate.Amount, Is.EqualTo(expectedUpdate.Amount));
                Assert.That(inventoryUpdate.ItemID, Is.EqualTo(expectedUpdate.ItemID));
            });
            
            Assert.Multiple(() =>
            {
                Assert.That(itemInfo.ItemID, Is.EqualTo(expectedItemInfo.ItemID));
                Assert.That(itemInfo.Amount, Is.EqualTo(expectedItemInfo.Amount));
                Assert.That(itemInfo.BaseSellPrice, Is.EqualTo(expectedItemInfo.BaseSellPrice));
            });
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
            
            Assert.Multiple(() =>
            {
                Assert.That(_inventoryUpdateErrorListener.WasCalled, Is.False);
                Assert.That(_inventoryUpdateResponseListener.WasCalled, Is.True);
            });

            InventoryUpdateResponse response = _inventoryUpdateResponseListener.InventoryUpdateResponse;
            Assert.That(response.InventoryUpdateEntries, Has.Length.EqualTo(1));
            InventoryUpdateEntry entry = response.InventoryUpdateEntries[0];
        
            Assert.That(entry.MutateType, Is.EqualTo(MutateType.CREATED));
            AssertResponseEntry(entry, _addStoneUpdate, _stoneInfo);
        }

        [Test]
        public void Positive_SendMultipleAddUpdates_CreatesMultipleItems_DispatchesCreationUpdate()
        {
            InventoryUpdate addCopper = _addStoneUpdate with { ItemID = ItemID.COPPER };
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate, addCopper));
            
            Assert.Multiple(() =>
            {
                Assert.That(_inventoryUpdateErrorListener.WasCalled, Is.False);
                Assert.That(_inventoryUpdateResponseListener.WasCalled, Is.True);
            });

            Assert.Multiple(() =>
            {
                InventoryUpdateResponse response = _inventoryUpdateResponseListener.InventoryUpdateResponse;
                Assert.That(response.InventoryUpdateEntries, Has.Length.EqualTo(2));
                
                foreach (InventoryUpdateEntry inventoryUpdateEntry in response.InventoryUpdateEntries)
                {
                    if (inventoryUpdateEntry.InventoryUpdate.ItemID == addCopper.ItemID)
                    {
                        AssertResponseEntry(inventoryUpdateEntry, addCopper, _stoneInfo with { ItemID = ItemID.COPPER });
                    }
                    else
                    {
                        AssertResponseEntry(inventoryUpdateEntry, _addStoneUpdate, _stoneInfo);
                    }
                }
            });
        }

        [Test]
        public void Positive_SendAddAfterCreation_ResponsesWithChanged()
        {
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));

            Assert.Multiple(() =>
            {
                Assert.That(_inventoryUpdateErrorListener.WasCalled, Is.False);
                Assert.That(_inventoryUpdateResponseListener.WasCalled, Is.True);
            });

            InventoryUpdateResponse response = _inventoryUpdateResponseListener.InventoryUpdateResponse;
            Assert.That(response.InventoryUpdateEntries, Has.Length.EqualTo(1));
            InventoryUpdateEntry entry = response.InventoryUpdateEntries[0];
            
            Assert.That(entry.MutateType, Is.EqualTo(MutateType.CHANGED));
            AssertResponseEntry(entry, _addStoneUpdate, _stoneInfo with { Amount = 20 });
        }

        [Test]
        public void Positive_SendRemoveAfterCreation_ResponsesWithDeleted()
        {
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE }));
            
            Assert.Multiple(() =>
            {
                Assert.That(_inventoryUpdateErrorListener.WasCalled, Is.False);
                Assert.That(_inventoryUpdateResponseListener.WasCalled, Is.True);
            });

            InventoryUpdateResponse response = _inventoryUpdateResponseListener.InventoryUpdateResponse;
            Assert.That(response.InventoryUpdateEntries, Has.Length.EqualTo(1));
            InventoryUpdateEntry entry = response.InventoryUpdateEntries[0];
            
            Assert.That(entry.MutateType, Is.EqualTo(MutateType.DELETED));
            AssertResponseEntry(entry, _addStoneUpdate with { ActionType = ActionType.REMOVE }, _stoneInfo with { Amount = 0, BaseSellPrice = 0 });
        }

        [Test]
        public void Positive_SendMixedUpdate_SingleItem_ResponsesWithCorrectAmount()
        { 
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE }, _addStoneUpdate, _addStoneUpdate));
            
            Assert.Multiple(() =>
            {
                Assert.That(_inventoryUpdateErrorListener.WasCalled, Is.False);
                Assert.That(_inventoryUpdateResponseListener.WasCalled, Is.True);
            });
            
            InventoryUpdateResponse response = _inventoryUpdateResponseListener.InventoryUpdateResponse;
            Assert.That(response.InventoryUpdateEntries, Has.Length.EqualTo(1));
            InventoryUpdateEntry entry = response.InventoryUpdateEntries[0];
            
            Assert.That(entry.MutateType, Is.EqualTo(MutateType.CREATED));
            AssertResponseEntry(entry, _addStoneUpdate, _stoneInfo with { Amount = 10 });
        }

        [Test]
        public void Positive_SendMixedUpdates_TwoItems_OneZero_DispatchesOneResponse()
        {
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE }, _addStoneUpdate, _addStoneUpdate with { ItemID = ItemID.COPPER }));
            
            Assert.Multiple(() =>
            {
                Assert.That(_inventoryUpdateErrorListener.WasCalled, Is.False);
                Assert.That(_inventoryUpdateResponseListener.WasCalled, Is.True);
            });
            
            InventoryUpdateResponse response = _inventoryUpdateResponseListener.InventoryUpdateResponse;
            Assert.That(response.InventoryUpdateEntries, Has.Length.EqualTo(1));
            InventoryUpdateEntry entry = response.InventoryUpdateEntries[0];
            
            Assert.That(entry.MutateType, Is.EqualTo(MutateType.CREATED));
            AssertResponseEntry(entry, _addStoneUpdate with { ItemID = ItemID.COPPER }, _stoneInfo with { Amount = 10, ItemID = ItemID.COPPER });
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
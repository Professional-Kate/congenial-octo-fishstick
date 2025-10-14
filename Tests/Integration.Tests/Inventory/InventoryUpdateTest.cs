using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Error;
using IdelPog.Inventory.Contracts.Response;
using IdelPog.Inventory.Exceptions;

namespace IdelPog.Integration.Tests.Inventory
{
    [TestFixture]
    public sealed class InventoryUpdateTest : ManagedTestBuffer
    {
        private ManagedResponseListener<InventoryUpdateResponse> _inventoryUpdateResponseListener;
        private ManagedErrorListener<InventoryUpdateError> _inventoryUpdateErrorListener;

        private InventoryUpdate _addStoneUpdate;
        private ItemInfo _stoneInfo;
        private ItemDefinitionCreation _stoneDefinition;

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
            
            _stoneDefinition = new ItemDefinitionCreation { ItemID = ItemID.STONE, BaseSellPrice = 1, Information = new Information { Name = "Stone", Description = "Stoney" } };
        }
        
        [SetUp]
        public void Setup()
        {
            _inventoryUpdateResponseListener = new ManagedResponseListener<InventoryUpdateResponse>();
            _inventoryUpdateErrorListener = new ManagedErrorListener<InventoryUpdateError>();
            
            ManagedSubscribe(_inventoryUpdateResponseListener);
            ManagedSubscribe(_inventoryUpdateErrorListener);
        }

        private void DispatchItemDefinitionCreations(params ItemDefinitionCreation[] creations)
        {
            IBuffer<ItemDefinitionCreation> buffer = BufferManager.RequestBuffer<ItemDefinitionCreation>(new BufferRequest(creations.Length));
            buffer.Assign(creations);
            buffer.MarkReady();
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
            Assert.That(_inventoryUpdateResponseListener.Responses, Has.Length.EqualTo(length));
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
            Assert.That(_inventoryUpdateErrorListener.Error.InventoryUpdates, Has.Length.EqualTo(length));
        }

        private void AssertResponseError<TException>()
        {
            InventoryUpdateError updateError = _inventoryUpdateErrorListener.Error;
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
            DispatchItemDefinitionCreations(_stoneDefinition);
                
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_inventoryUpdateResponseListener.Responses[0], _stoneInfo, MutateType.CREATED);
        }

        [Test]
        public void Positive_SendMultipleAddUpdates_CreatesMultipleItems_DispatchesCreationUpdate()
        {
            InventoryUpdate addCopper = _addStoneUpdate with { ItemID = ItemID.COPPER };
            DispatchItemDefinitionCreations(_stoneDefinition, _stoneDefinition with { ItemID = ItemID.COPPER });
            
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate, addCopper));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(2);
            AssertResponse(_inventoryUpdateResponseListener.Responses[0], _stoneInfo, MutateType.CREATED);
            AssertResponse(_inventoryUpdateResponseListener.Responses[1], _stoneInfo with { ItemID = ItemID.COPPER }, MutateType.CREATED);
            
        }

        [Test]
        public void Positive_SendAddAfterCreation_ResponsesWithChanged()
        {
            DispatchItemDefinitionCreations(_stoneDefinition);
            
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));

            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_inventoryUpdateResponseListener.Responses[0], _stoneInfo with { Amount = 20 }, MutateType.CHANGED);
        }

        [Test]
        public void Positive_SendRemoveAfterCreation_ResponsesWithDeleted()
        {
            DispatchItemDefinitionCreations(_stoneDefinition);
            
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE }));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_inventoryUpdateResponseListener.Responses[0], _stoneInfo with { Amount = 0 }, MutateType.DELETED);
        }

        [Test]
        public void Positive_SendMixedUpdate_SingleItem_ResponsesWithCorrectAmount()
        { 
            DispatchItemDefinitionCreations(_stoneDefinition);
            
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE }, _addStoneUpdate, _addStoneUpdate));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_inventoryUpdateResponseListener.Responses[0], _stoneInfo, MutateType.CREATED);
        }

        [Test]
        public void Positive_SendMixedUpdates_TwoItems_OneZero_DispatchesOneResponse()
        {
            DispatchItemDefinitionCreations(_stoneDefinition, _stoneDefinition with { ItemID = ItemID.COPPER });
            
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE }, _addStoneUpdate, _addStoneUpdate with { ItemID = ItemID.COPPER }));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(false);
            AssertResponseLength(1);
            AssertResponse(_inventoryUpdateResponseListener.Responses[0], _stoneInfo with { ItemID = ItemID.COPPER }, MutateType.CREATED);
        }

        [Test]
        public void Negative_SendMixedUpdates_OneItem_AmountZero_DispatchesError()
        {
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE }, _addStoneUpdate));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(2);
            AssertResponseError<EmptyCollectionException>();
        }

        [Test]
        public void Negative_SendRemoveUpdate_ItemNotCreated_DispatchesError()
        {
            DispatchItemDefinitionCreations(_stoneDefinition);
            
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE }));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertResponseError<NotFoundException<ItemID>>();
        }

        [Test]
        public void Negative_SendMixedUpdates_FinalAmountNegative_DispatchesError()
        {
            DispatchItemDefinitionCreations(_stoneDefinition);
            
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = ActionType.REMOVE, Amount = 11 }));
            
            AssertResponseListenerCalled(true);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertResponseError<InsufficientAmountException>();
        }

        [Test]
        public void Negative_SendBadActionType_DispatchesError()
        {
            DispatchItemDefinitionCreations(_stoneDefinition);
            
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate with { ActionType = (ActionType) 5 }));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertResponseError<ArgumentOutOfRangeException>();
        }

        [Test]
        public void Negative_SendUpdate_NoDefinitionFound_DispatchesError()
        {
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));
            
            AssertResponseListenerCalled(false);
            AssertErrorListenerCalled(true);
            AssertErrorLength(1);
            AssertResponseError<NotFoundException<ItemID>>();
        }
    }
}
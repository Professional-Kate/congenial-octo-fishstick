using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Messaging.Buffer;

namespace IdelPog.Integration.Tests.Inventory
{
    [TestFixture]
    public class InventoryUpdateTest : ManagedBuffer
    {
        private InventoryUpdateResponseListener _inventoryUpdateResponseListener;
        private InventoryUpdateErrorListener _inventoryUpdateErrorListener;

        private InventoryUpdate _addStoneUpdate;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _addStoneUpdate = new InventoryUpdate
            {
                Amount = 1,
                Action = ActionType.ADD,
                ItemID = ItemID.STONE
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

        [Test]
        public void Positive_SendAddUpdate_CreatesStone_DispatchesCreationUpdate()
        {
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate));
            
            Assert.Multiple(() =>
            {
                Assert.That(_inventoryUpdateErrorListener.WasCalled, Is.False);
                Assert.That(_inventoryUpdateResponseListener.WasCalled, Is.True);
            });

            Assert.Multiple(() =>
            {
                InventoryUpdateResponse response = _inventoryUpdateResponseListener.InventoryUpdateResponse;
                Assert.That(response.InventoryUpdateEntry, Has.Length.EqualTo(1));
                InventoryUpdateEntry entry = response.InventoryUpdateEntry[0];
                
                Assert.That(entry.MutateType, Is.EqualTo(MutateType.CREATED));
                Assert.That(entry.InventoryUpdate.Action, Is.EqualTo(ActionType.ADD));
                
                ItemInfo itemInfo = entry.ItemInfo;
                Assert.That(itemInfo.ItemID, Is.EqualTo(_addStoneUpdate.ItemID));
                Assert.That(itemInfo.Amount, Is.EqualTo(_addStoneUpdate.Amount));
            });
        }

        [Test]
        public void Positive_SendMultipleAddUpdates_CreatesMultipleItems_DispatchesCreationUpdate()
        {
            InventoryUpdate addCopper = _addStoneUpdate with { ItemID = ItemID.COPPER };
            Assert.DoesNotThrow(() => DispatchInventoryUpdate(_addStoneUpdate, _addStoneUpdate, addCopper));
            
            Assert.Multiple(() =>
            {
                Assert.That(_inventoryUpdateErrorListener.WasCalled, Is.False);
                Assert.That(_inventoryUpdateResponseListener.WasCalled, Is.True);
            });

            Assert.Multiple(() =>
            {
                InventoryUpdateResponse response = _inventoryUpdateResponseListener.InventoryUpdateResponse;
                Assert.That(response.InventoryUpdateEntry, Has.Length.EqualTo(2));
            });
        }
    }
}
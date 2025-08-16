using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Assertion;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Core.Validation.Handler;
using IdelPog.Inventory.Factory.Interface;
using IdelPog.Inventory.Service;
using IdelPog.Inventory.Service.Interface;
using Moq;

namespace IdelPog.Inventory.Tests
{
    [TestFixture]
    public class InventoryUpdateSummarizerTest
    {
        private IInventoryUpdateSummarizer _inventoryUpdateSummarizer;
        private Mock<IInventoryUpdateFactory> _inventoryUpdateFactoryMock;

        private InventoryUpdate _addCopperUpdate;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _addCopperUpdate = new InventoryUpdate
            {
                Action = ActionType.ADD,
                Amount = 1,
                ItemID = ItemID.COPPER
            };
            
            _inventoryUpdateFactoryMock = new Mock<IInventoryUpdateFactory>();
            
            _inventoryUpdateSummarizer = new InventoryUpdateSummarizer(_inventoryUpdateFactoryMock.Object, new CollectionAssertion(new ThrowHandler()));
        }

        [SetUp]
        public void Setup()
        {
            _inventoryUpdateFactoryMock.Reset();
        }

        private void SetupFactory(InventoryUpdate expectedUpdate, InventoryUpdate returnValue)
        {
            _inventoryUpdateFactoryMock.Setup(library => library.Create(expectedUpdate.ItemID, expectedUpdate.Amount, expectedUpdate.Action)).Returns(returnValue);
        }

        private void VerifyFactoryCalls(int amountOfCalls)
        {
            _inventoryUpdateFactoryMock.Verify(library => library.Create(It.IsAny<ItemID>(), It.IsAny<uint>(), It.IsAny<ActionType>()), Times.Exactly(amountOfCalls));
        }

        [Test]
        public void Positive_GetSummary_OneItem_OnePassedUpdate_ReturnsOneUpdate()
        {
            SetupFactory(_addCopperUpdate, _addCopperUpdate);
            
            InventoryUpdate[] updates = _inventoryUpdateSummarizer.GetSummary([_addCopperUpdate]);
            
            Assert.Multiple(() =>
            { 
                Assert.That(updates, Has.Length.EqualTo(1)); 
                Assert.That(updates[0], Is.EqualTo(_addCopperUpdate));
            });

            VerifyFactoryCalls(1);
        }

        [Test]
        public void Positive_GetSummary_OneItem_MultipleUpdates_ReturnsOneUpdate()
        {
            InventoryUpdate expectedUpdate = _addCopperUpdate with { Amount = 3 };
            SetupFactory(expectedUpdate, expectedUpdate);
            
            InventoryUpdate[] updates = _inventoryUpdateSummarizer.GetSummary([_addCopperUpdate, _addCopperUpdate, _addCopperUpdate]);
            
            Assert.Multiple(() =>
            { 
                Assert.That(updates, Has.Length.EqualTo(1)); 
                Assert.That(updates[0], Is.EqualTo(expectedUpdate));
            });
            
            VerifyFactoryCalls(1);
        }

        [Test]
        public void Positive_GetSummary_OneItem_FinalAmountIsZero_ReturnsNothing()
        {
            InventoryUpdate removeCopperUpdate = _addCopperUpdate with { Action = ActionType.REMOVE };
            
            InventoryUpdate[] updates = _inventoryUpdateSummarizer.GetSummary([removeCopperUpdate, _addCopperUpdate]);
            
            Assert.That(updates, Has.Length.EqualTo(0)); 
            VerifyFactoryCalls(0);
        }

        [Test]
        public void Positive_GetSummary_MultipleItems_ReturnsMultipleUpdates()
        {
            InventoryUpdate addStoneUpdate = _addCopperUpdate with { ItemID = ItemID.STONE };
            SetupFactory(addStoneUpdate, addStoneUpdate);
            SetupFactory(_addCopperUpdate, _addCopperUpdate);
            
            InventoryUpdate[] updates = _inventoryUpdateSummarizer.GetSummary([addStoneUpdate, _addCopperUpdate]);
            
            Assert.That(updates, Has.Length.EqualTo(2)); 
            foreach (InventoryUpdate inventoryUpdate in updates)
            {
                Assert.That(inventoryUpdate, inventoryUpdate.ItemID == ItemID.STONE ? Is.EqualTo(addStoneUpdate) : Is.EqualTo(_addCopperUpdate));
            }
            
            VerifyFactoryCalls(2);
        }

        [Test]
        public void Negative_GetSummary_NullCollection_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _inventoryUpdateSummarizer.GetSummary(null!));
        }

        [Test]
        public void Negative_GetSummary_EmptyCollection_Throws()
        {
            Assert.Throws<EmptyCollectionException>(() => _inventoryUpdateSummarizer.GetSummary([]));
        }
    }
}
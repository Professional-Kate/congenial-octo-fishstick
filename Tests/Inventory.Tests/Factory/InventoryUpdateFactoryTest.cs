using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Inventory.Factory;

namespace IdelPog.Inventory.Tests.Factory
{
    [TestFixture]
    public sealed class InventoryUpdateFactoryTest
    {
        private InventoryUpdateFactory _updateFactory;
        private InventoryUpdate _honeyUpdate;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _honeyUpdate = new InventoryUpdate { ItemID = ItemID.HONEY, Amount = 1, ActionType = ActionType.ADD };
            
            _updateFactory = new InventoryUpdateFactory();
        }

        private static void AssertFactoryResponse(InventoryUpdate response, InventoryUpdate source)
        { 
            Assert.That(response, Is.EqualTo(source));
        }

        [Test]
        public void Positive_Create_ReturnsExpectedUpdate()
        {
            InventoryUpdate update = _updateFactory.Create(_honeyUpdate.ItemID, _honeyUpdate.Amount, _honeyUpdate.ActionType);

            AssertFactoryResponse(update, _honeyUpdate);
        }

        [TestCase(1u)]
        [TestCase(2u)]
        public void Positive_CreateMultiple_CreatesExpectedUpdates(uint iterations)
        {
            InventoryUpdate[] updates = _updateFactory.CreateMultiple(_honeyUpdate.ItemID, _honeyUpdate.Amount, _honeyUpdate.ActionType, iterations);

            Assert.That(updates, Has.Length.EqualTo(iterations));
            foreach (InventoryUpdate inventoryUpdate in updates)
            {
                AssertFactoryResponse(inventoryUpdate, _honeyUpdate);
            }
        }

        [Test]
        public void Positive_CreateMultiple_ZeroIterations_CreatesNothing()
        {
            InventoryUpdate[] updates = _updateFactory.CreateMultiple(_honeyUpdate.ItemID, _honeyUpdate.Amount, _honeyUpdate.ActionType, 0);

            Assert.Multiple(() =>
            {
                Assert.That(updates, Has.Length.EqualTo(0));
                Assert.That(updates, Is.Empty);
            });
        }
    }
}
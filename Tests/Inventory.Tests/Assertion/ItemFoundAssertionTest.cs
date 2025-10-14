using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Exceptions;
using IdelPog.Inventory.Assertion;

namespace IdelPog.Inventory.Tests.Assertion
{
    [TestFixture]
    public sealed class ItemFoundAssertionTest
    {
        private ItemFoundAssertion _itemFoundAssertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _itemFoundAssertion = new ItemFoundAssertion();
        }

        [Test]
        public void Positive_AssertItemFound_ActionTypeIsNotRemove_NoThrow()
        {
            Assert.DoesNotThrow(() => _itemFoundAssertion.AssertItemFound(false, ActionType.ADD, ItemID.BIRCH));
        }

        [Test]
        public void Positive_AssertItemFound_ContainsTrue_NoThrow()
        {
            Assert.DoesNotThrow(() => _itemFoundAssertion.AssertItemFound(true, ActionType.REMOVE, ItemID.BIRCH));
        }

        [Test]
        public void Negative_AssertItemFound_ContainsFalse_Throws()
        {
            NotFoundException<ItemID> exception = Assert.Throws<NotFoundException<ItemID>>(() => _itemFoundAssertion.AssertItemFound(false, ActionType.REMOVE, ItemID.BIRCH));
            
            Assert.That(exception.Key, Is.EqualTo(ItemID.BIRCH));
        }
    }
}
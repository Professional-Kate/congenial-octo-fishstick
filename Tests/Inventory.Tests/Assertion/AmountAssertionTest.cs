using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Handler;
using IdelPog.Inventory.Assertion;
using IdelPog.Inventory.Assertion.Interface;
using IdelPog.Inventory.Exceptions;

namespace IdelPog.Inventory.Tests.Assertion
{
    [TestFixture]
    public class AmountAssertionTest
    {
        private IAmountAssertion _amountAssertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _amountAssertion = new AmountAssertion(new ThrowHandler());
        }

        private void AssertionTestRunner(uint requestedAmount, uint actualAmount)
        {
            _amountAssertion.AssertEnoughAmount(requestedAmount, actualAmount, ItemID.STONE);
        }

        [Test]
        public void Positive_AmountIsGreater_NoThrow()
        {
            Assert.DoesNotThrow(() => AssertionTestRunner(1, 10));
        }

        [Test]
        public void Positive_AmountIsTheSame_NoThrow()
        {
            Assert.DoesNotThrow(() => AssertionTestRunner(10u, 10u));
        }

        [Test]
        public void Negative_AmountIsLesser_Throws()
        {
            InsufficientAmountException exception = Assert.Throws<InsufficientAmountException>(() => AssertionTestRunner(10, 1));
            
            Assert.Multiple(() =>
            {
                Assert.That(exception.ItemID, Is.EqualTo(ItemID.STONE));
                Assert.That(exception.RequestedAmount, Is.EqualTo(10));
                Assert.That(exception.ActualAmount, Is.EqualTo(1));
                Assert.That(exception.Message, Is.Not.Null.Or.Empty);
            });
        }
    }
}
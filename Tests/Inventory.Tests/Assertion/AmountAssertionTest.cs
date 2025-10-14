using IdelPog.Core.Contracts.Enum;
using IdelPog.Inventory.Assertion;
using IdelPog.Inventory.Exceptions;

namespace IdelPog.Inventory.Tests.Assertion
{
    [TestFixture]
    public sealed class AmountAssertionTest
    {
        private AmountAssertion _amountAssertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _amountAssertion = new AmountAssertion();
        }

        private void AssertionTestRunner(uint requestedAmount, uint actualAmount)
        {
            _amountAssertion.AssertEnoughAmount(requestedAmount, actualAmount, ItemID.STONE);
        }

        [Test]
        public void Positive_AssertEnoughAmount_AmountIsGreater_NoThrow()
        {
            Assert.DoesNotThrow(() => AssertionTestRunner(1, 10));
        }

        [Test]
        public void Positive_AssertEnoughAmount_AmountIsTheSame_NoThrow()
        {
            Assert.DoesNotThrow(() => AssertionTestRunner(10u, 10u));
        }

        [Test]
        public void Negative_AssertEnoughAmount_AmountIsLesser_Throws()
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
        
        [Test]
        public void Positive_AssertAmountNotZero_AmountNotZero_NoThrow()
        {
            Assert.DoesNotThrow(() => _amountAssertion.AssertAmountNotZero(1));
        }
        
        [Test]
        public void Positive_AssertAmountNotZero_AmountZero_Throws()
        {
            Assert.Throws<AmountZeroException>(() => _amountAssertion.AssertAmountNotZero(0));
        }
    }
}
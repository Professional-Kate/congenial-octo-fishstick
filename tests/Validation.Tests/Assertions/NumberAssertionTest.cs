using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.Validation.Tests.Assertions
{
    [TestFixture]
    public class NumberAssertionTest
    {
        private INumberAssertion _numberAssertion { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _numberAssertion = new NumberAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertNonNegative_PositiveNumber_NoThrow()
        {
            Assert.DoesNotThrow(() => _numberAssertion.AssertNonNegative(100));
        }

        [Test]
        public void Positive_AssertAllNonNegative_PositiveNumberArray_NoThrow()
        {
            Assert.DoesNotThrow(() => _numberAssertion.AssertAllNonNegative([1, 2, 3]));
        }

        [Test]
        public void Negative_AssertNonNegative_NegativeNumber_Throws()
        {
            Assert.Throws<NegativeNumberException>(() => _numberAssertion.AssertNonNegative(-100));
        }

        [Test]
        public void Negative_AssertAllNonNegative_OneNegativeNumber_Throws()
        {
            Assert.Throws<NegativeNumberException>(() => _numberAssertion.AssertAllNonNegative([10, 20, 30, -1]));
        }
    }
}
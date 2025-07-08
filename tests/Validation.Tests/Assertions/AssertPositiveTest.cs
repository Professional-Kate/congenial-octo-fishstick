using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.Validation.Tests.Assertions
{
    [TestFixture]
    public class AssertPositiveTest
    {
        private IAssertPositive _assertPositive { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _assertPositive = new AssertPositive(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertNumberIsPositive_PositiveNumber()
        {
            Assert.DoesNotThrow(() => _assertPositive.AssertNumberIsPositive<int>(100));
        }
        
        [Test]
        public void Positive_AssertNumberIsPositive_Array_PositiveNumber()
        {
            Assert.DoesNotThrow(() => _assertPositive.AssertNumberIsPositive<int>(1, 2, 3));
        }

        [Test]
        public void Negative_AssertNumberIsPositive_NegativeNumber_Throws()
        {
            Assert.Throws<NegativeNumberException>(() => _assertPositive.AssertNumberIsPositive<int>(-100));
        }
        
        [Test]
        public void Negative_AssertNumberIsPositive_OneNegativeNumber_Throws()
        {
            Assert.Throws<NegativeNumberException>(() => _assertPositive.AssertNumberIsPositive<int>(10, 20, 30, -1));
        }
    }
}
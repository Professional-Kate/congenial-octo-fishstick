using IdelPogTemp.Main.Validation.Assertions;
using IdelPogTemp.Main.Validation.Assertions.Handlers;
using IdelPogTemp.Main.Validation.Assertions.Interfaces;
using IdelPogTemp.Main.Validation.Exceptions;
using NUnit.Framework;

namespace IdelPogTemp.Tests.Validation
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
            Assert.DoesNotThrow(() => _assertPositive.AssertNumberIsPositive(100));
        }
        
        [Test]
        public void Positive_AssertNumberIsPositive_Array_PositiveNumber()
        {
            Assert.DoesNotThrow(() => _assertPositive.AssertNumberIsPositive(1, 2, 3));
        }

        [Test]
        public void Negative_AssertNumberIsPositive_NegativeNumber_Throws()
        {
            Assert.Throws<NegativeNumberException>(() => _assertPositive.AssertNumberIsPositive(-100));
        }
        
        [Test]
        public void Negative_AssertNumberIsPositive_OneNegativeNumber_Throws()
        {
            Assert.Throws<NegativeNumberException>(() => _assertPositive.AssertNumberIsPositive(10, 20, 30, -1));
        }
    }
}
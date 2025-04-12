using IdelPog.Main.Validation.Assertions;
using IdelPog.Main.Validation.Assertions.Handlers;
using IdelPog.Main.Validation.Assertions.Interfaces;
using NUnit.Framework;

namespace IdelPog.Tests.Validation
{
    [TestFixture]
    public class AssertNotNullTest
    {
        private IAssertNotNull _assertNotNull { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _assertNotNull = new AssertNotNull(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertObjectNotNull_NotNull()
        {
            Assert.DoesNotThrow(() => _assertNotNull.AssertObjectNotNull(10));
        }

        [Test]
        public void Negative_AssertObjectNotNull_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _assertNotNull.AssertObjectNotNull(null));
        }
    }
}
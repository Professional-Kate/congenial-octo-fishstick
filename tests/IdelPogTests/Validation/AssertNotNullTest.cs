using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPogTests.Validation
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
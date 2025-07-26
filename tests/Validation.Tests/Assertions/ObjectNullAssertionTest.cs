using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.Validation.Tests.Assertions
{
    [TestFixture]
    public class ObjectNullAssertionTest
    {
        private IObjectNullAssertion _objectNullAssertion { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _objectNullAssertion = new ObjectNullAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertObjectNotNull_NotNull()
        {
            Assert.DoesNotThrow(() => _objectNullAssertion.AssertNotNull(10, "int"));
        }

        [Test]
        public void Negative_AssertObjectNotNull_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _objectNullAssertion.AssertNotNull<string>(null, "null"));
        }
    }
}
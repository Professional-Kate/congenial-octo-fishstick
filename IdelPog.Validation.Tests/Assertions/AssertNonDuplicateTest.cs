using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Exceptions;

namespace IdelPog.Validation.Tests.Assertions
{
    [TestFixture]
    public class AssertNonDuplicateTest
    {
        private IAssertNonDuplicate _assertNonDuplicate { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _assertNonDuplicate = new AssertNonDuplicate(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertUnique_PassedFalse()
        {
            Assert.DoesNotThrow(() => _assertNonDuplicate.AssertContains(10, () => false));
        }

        [Test]
        public void Negative_AssertUnique_PassedTrue_Throws()
        {
            Assert.Throws<DuplicateItemException>(() => _assertNonDuplicate.AssertContains(10, () => true));
        }
    }
}
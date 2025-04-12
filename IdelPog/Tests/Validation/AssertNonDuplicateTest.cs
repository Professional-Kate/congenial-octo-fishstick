using IdelPogTemp.Main.Validation.Assertions;
using IdelPogTemp.Main.Validation.Assertions.Handlers;
using IdelPogTemp.Main.Validation.Assertions.Interfaces;
using IdelPogTemp.Main.Validation.Exceptions;
using NUnit.Framework;

namespace IdelPogTemp.Tests.Validation
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
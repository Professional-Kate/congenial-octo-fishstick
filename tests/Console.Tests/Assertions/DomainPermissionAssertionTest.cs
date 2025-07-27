using Console.Assertions;
using Console.Exceptions;
using Console.Types;
using IdelPog.Validation.Assertions.Handlers;

namespace Console.Tests.Assertions
{
    [TestFixture]
    public class DomainPermissionAssertionTest
    {
        private IDomainPermissionAssertion _assertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _assertion = new DomainPermissionAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertHasPermission_PassesTrue_NotThrow()
        {
            Assert.DoesNotThrow(() => _assertion.AssertHasPermission(true, Domain.SCHEDULE));
        }

        [Test]
        public void Negative_AssertHasPermission_PassesFalse_Throws()
        {
            Assert.Throws<DomainPermissionDeniedException>(() => _assertion.AssertHasPermission(false, Domain.SCHEDULE));
        }
    }
}
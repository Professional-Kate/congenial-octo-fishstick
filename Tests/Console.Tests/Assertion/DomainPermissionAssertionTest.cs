using IdelPog.Console.Assertion;
using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Exceptions;
using IdelPog.Console.Types;
using IdelPog.Core.Validation.Handler;

namespace IdelPog.Console.Tests.Assertion
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
            Assert.DoesNotThrow(() => _assertion.AssertHasPermission(true, Domain.CURRENCY));
        }

        [Test]
        public void Negative_AssertHasPermission_PassesFalse_Throws()
        {
            Assert.Throws<DomainPermissionDeniedException>(() => _assertion.AssertHasPermission(false, Domain.CURRENCY));
        }
    }
}
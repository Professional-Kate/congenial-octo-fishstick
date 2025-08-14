using IdelPog.Console.Assertion;
using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Exceptions;
using IdelPog.Core.Validation.Handler;

namespace IdelPog.Console.Tests.Assertion
{
    [TestFixture]
    public class TypeParseAssertionTest
    {
        private ITypeParseAssertion _assertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _assertion = new TypeParseAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertCanParse_PassesTrue_NoThrow()
        {
            Assert.DoesNotThrow(() => _assertion.AssertCanParse(true, "12", typeof(int)));
        }

        [Test]
        public void Positive_AssertCanParse_PassesFalse_Throws()
        {
            Assert.Throws<FailedTypeParseException>(() => _assertion.AssertCanParse(false, "12", typeof(int)));
        }
    }
}
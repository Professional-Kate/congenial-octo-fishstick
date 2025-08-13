using IdelPog.Console.Assertion;
using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Exceptions;
using IdelPog.Core.Validation.Handler;

namespace IdelPog.Console.Tests.Assertion
{
    [TestFixture]
    public class EnumParseAssertionTest
    {
        private IEnumParseAssertion _assertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _assertion = new EnumParseAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertCanParse_PassesTrue_NoThrow()
        {
            Assert.DoesNotThrow(() => _assertion.AssertCanParse(true, "hello", "there"));
        }

        [Test]
        public void Negative_AssertCanParse_PassesFalse_Throws()
        {
            Assert.Throws<FailedEnumParseException>(() => _assertion.AssertCanParse(false, "hello", "there"));
        }
    }
}
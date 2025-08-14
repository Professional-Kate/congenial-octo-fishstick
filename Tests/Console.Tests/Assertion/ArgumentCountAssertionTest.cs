using IdelPog.Console.Assertion;
using IdelPog.Console.Exceptions;
using IdelPog.Core.Validation.Handler;

namespace IdelPog.Console.Tests.Assertion
{
    [TestFixture]
    public class ArgumentCountAssertionTest
    {
        private ArgumentCountAssertion _assertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _assertion = new ArgumentCountAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertCount_SameCount_NoThrow()
        {
            Assert.DoesNotThrow(() => _assertion.AssertCount(1, 1));
        }

        [Test]
        public void Negative_AssertCount_DifferentCount_Throws()
        {
            Assert.Throws<InvalidArgumentCountException>(() => _assertion.AssertCount(1, 2));
        }
    }
}
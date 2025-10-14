using IdelPog.Console.Assertion;
using IdelPog.Console.Exceptions;

namespace IdelPog.Console.Tests.Assertion
{
    [TestFixture]
    public sealed class ArgumentCountAssertionTest
    {
        private ArgumentCountAssertion _assertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _assertion = new ArgumentCountAssertion();
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
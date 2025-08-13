using IdelPog.Console.Assertion;
using IdelPog.Console.Assertion.Interface;
using IdelPog.Console.Runtime.Input.Exceptions;
using IdelPog.Core.Validation.Handler;

namespace IdelPog.Console.Tests.Assertion
{
    [TestFixture]
    public class SpanAssertionTest
    {
        private ISpanAssertion _spanAssertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _spanAssertion = new SpanAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertNotEmpty_PassesNonEmptySpan_NoThrow()
        {
            Assert.DoesNotThrow(() => _spanAssertion.AssertNotEmpty(new ReadOnlySpan<int>([1, 2, 3])));
        }

        [Test]
        public void Negative_AssertNotEmpty_PassesEmptySpan_Throws()
        {
            Assert.Throws<EmptySpanException>(() => _spanAssertion.AssertNotEmpty(new ReadOnlySpan<int>([])));
        }
    }
}
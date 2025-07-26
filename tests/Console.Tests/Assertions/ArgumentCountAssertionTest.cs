using Console.Assertions;
using Console.Commands.Resolver.Exceptions;
using IdelPog.Validation.Assertions.Handlers;

namespace Console.Tests.Assertions
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
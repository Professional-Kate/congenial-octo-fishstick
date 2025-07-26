using Console.Assertions;
using Console.Commands.Resolver.Exceptions;
using IdelPog.Validation.Assertions.Handlers;

namespace Console.Tests.Assertions
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
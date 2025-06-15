using IdelPog.Staging.Assertions;
using IdelPog.Staging.Exceptions;
using IdelPog.Staging.Tests.Messaging;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.Staging.Tests.Assertions
{
    [TestFixture]
    public class AssertListenerFoundTest
    {
        private IAssertListenerFound _assertListenerFound { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _assertListenerFound = new AssertListenerFound(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertFound_True_NoThrow()
        {
            Assert.DoesNotThrow(() => _assertListenerFound.AssertFound(new TestListener<int>(), true));
        }

        [Test]
        public void Negative_AssertFound_False_Throws()
        {
            Assert.Throws<NoListenerFoundException>(() => _assertListenerFound.AssertFound(new TestListener<int>(), false));
        }
    }
}
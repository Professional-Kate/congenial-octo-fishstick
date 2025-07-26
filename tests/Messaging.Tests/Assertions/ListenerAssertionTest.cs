using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Exceptions;
using IdelPog.Messaging.Tests.Messaging;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.Messaging.Tests.Assertions
{
    [TestFixture]
    public class ListenerAssertionTest
    {
        private IListenerAssertion _listenerAssertion { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _listenerAssertion = new ListenerAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertFound_True_NoThrow()
        {
            Assert.DoesNotThrow(() => _listenerAssertion.AssertListenerFound(true, new TestListener<int>()));
        }

        [Test]
        public void Negative_AssertFound_False_Throws()
        {
            Assert.Throws<NoListenerFoundException>(() => _listenerAssertion.AssertListenerFound(false, new TestListener<int>()));
        }
    }
}
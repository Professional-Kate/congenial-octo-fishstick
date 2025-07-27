using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Exceptions;
using IdelPog.Messaging.Listeners.Single;
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
            IListener testListener = new TestListener<int>();
            NoListenerFoundException exception = Assert.Throws<NoListenerFoundException>(() => _listenerAssertion.AssertListenerFound(false, testListener));
            Assert.Multiple(() =>
            {
                Assert.That(exception.Listener, Is.EqualTo(testListener));
                Assert.That(exception.ListenerType, Is.EqualTo(typeof(int)));
            });
        }
    }
}
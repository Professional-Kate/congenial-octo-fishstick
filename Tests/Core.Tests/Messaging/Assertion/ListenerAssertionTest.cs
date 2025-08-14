using IdelPog.Core.Messaging.Assertion;
using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Messaging.Listener;
using IdelPog.Core.Tests.Messaging.Messaging;
using IdelPog.Core.Validation.Handler;

namespace IdelPog.Core.Tests.Messaging.Assertion
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
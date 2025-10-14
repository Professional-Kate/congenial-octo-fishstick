using IdelPog.Core.Messaging.Assertion;
using IdelPog.Core.Messaging.Exceptions;
using IdelPog.Core.Messaging.Messenger;
using IdelPog.Core.Validation.Assertion;

namespace IdelPog.Core.Tests.Messaging.Messaging
{
    [TestFixture]
    public sealed class BufferMessengerTest
    {
        private BufferMessenger _bufferMessenger { get; set; }
        private TestListener<int> _intListener { get; set; }

        private readonly IReadOnlyList<int> _bufferData = [1, 2, 3];

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            Setup();
            _intListener = new TestListener<int>();
        }

        [TearDown]
        public void TearDown()
        {
            Setup();
            _intListener.ResetObject();
        }

        private void Setup()
        {
            _bufferMessenger = new BufferMessenger(new ObjectNullAssertion(), new ListenerAssertion());
        }

        [Test]
        public void Positive_Subscribe_SubscribesListener()
        {
            _bufferMessenger.Subscribe(_intListener);

            _bufferMessenger.DispatchMessage(_bufferData);

            Assert.Multiple(() =>
            {
                Assert.That(_intListener.WasCalled, Is.True);
                Assert.That(_intListener.AmountCalled, Is.EqualTo(1));
            });
        }

        [Test]
        public void Positive_Subscribe_SameType()
        {
            _bufferMessenger.Subscribe(_intListener);
            _bufferMessenger.Subscribe(_intListener);

            _bufferMessenger.DispatchMessage(_bufferData);

            Assert.That(_intListener.AmountCalled, Is.EqualTo(2));
        }

        [Test]
        public void Positive_Subscribe_CanSubscribeSingleListener()
        {
            SingleTestListener<int> intListener = new();
            _bufferMessenger.Subscribe(intListener);

            _bufferMessenger.DispatchMessage([1]);

            Assert.That(intListener.WasCalled, Is.True);
        }

        [Test]
        public void Positive_DispatchMessage_DispatchesMessage()
        {
            _bufferMessenger.Subscribe(_intListener);
            _bufferMessenger.DispatchMessage(_bufferData);

            Assert.That(_intListener.BufferData, Is.EquivalentTo(_bufferData));
        }

        [Test]
        public void Positive_DispatchMessage_DispatchesCorrectMessage()
        {
            TestListener<uint> intListener = new();

            _bufferMessenger.Subscribe(_intListener);
            _bufferMessenger.Subscribe(intListener);

            _bufferMessenger.DispatchMessage([10u]);
            _bufferMessenger.DispatchMessage(_bufferData);

            Assert.Multiple(() =>
            {
                Assert.That(intListener.WasCalled, Is.True);
                Assert.That(_intListener.WasCalled, Is.True);
                Assert.That(intListener.AmountCalled, Is.EqualTo(1));
                Assert.That(_intListener.AmountCalled, Is.EqualTo(1));
            });
        }

        [Test]
        public void Positive_DispatchMessage_NoListener_NoThrow()
        {
            Assert.DoesNotThrow(() => _bufferMessenger.DispatchMessage(_bufferData));
        }

        [Test]
        public void Positive_DispatchMessage_DispatchesSingleMessage()
        {
            SingleTestListener<int> intListener = new();
            _bufferMessenger.Subscribe(intListener);
            _bufferMessenger.Subscribe(_intListener);

            IReadOnlyList<int> list = [1];

            _bufferMessenger.DispatchMessage(list);

            Assert.Multiple(() =>
            {
                Assert.That(intListener.WasCalled, Is.True);
                Assert.That(_intListener.WasCalled, Is.True);
                Assert.That(intListener.Data, Is.EqualTo(1));
                Assert.That(_intListener.BufferData, Is.EqualTo(list));
            });
        }

        [Test]
        public void Positive_DispatchMessage_NoSingleDispatch()
        {
            SingleTestListener<int> intListener = new();
            _bufferMessenger.Subscribe(intListener);
            _bufferMessenger.Subscribe(_intListener);

            _bufferMessenger.DispatchMessage([1, 2]);

            Assert.Multiple(() =>
            {
                Assert.That(intListener.WasCalled, Is.False);
                Assert.That(_intListener.WasCalled, Is.True);
            });
        }

        [Test]
        public void Positive_Unsubscribe_UnsubscribesListener()
        {
            _bufferMessenger.Subscribe(_intListener);
            _bufferMessenger.Unsubscribe(_intListener);

            _bufferMessenger.DispatchMessage(_bufferData);

            Assert.That(_intListener.WasCalled, Is.False);
        }

        [Test]
        public void Positive_Unsubscribe_UnsubscribesSingleListener()
        {
            SingleTestListener<int> intListener = new();
            _bufferMessenger.Subscribe(intListener);

            _bufferMessenger.Unsubscribe(intListener);

            _bufferMessenger.DispatchMessage(_bufferData);

            Assert.That(_intListener.WasCalled, Is.False);
        }

        [Test]
        public void Positive_MultipleListeners_FirstThrows_Continues()
        {
            TestListener<int> intListener = new()
            {
                ShouldThrowException = true
            };

            _bufferMessenger.Subscribe(_intListener);
            _bufferMessenger.Subscribe(intListener);

            Assert.DoesNotThrow(() => _bufferMessenger.DispatchMessage(_bufferData));
            Assert.Multiple(() =>
            {
                Assert.That(_intListener.AmountCalled, Is.EqualTo(1));
                Assert.That(intListener.AmountCalled, Is.EqualTo(1));
            });
        }

        [Test]
        public void Negative_Subscribe_NullListener_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _bufferMessenger.Subscribe(null!));
        }

        [Test]
        public void Negative_DispatchMessage_NullBuffer_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _bufferMessenger.DispatchMessage<int>(null!));
        }

        [Test]
        public void Negative_Unsubscribe_NullListener_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _bufferMessenger.Unsubscribe(null!));
        }

        [Test]
        public void Negative_Unsubscribe_NotSubscribed_Throws()
        {
            NoListenerFoundException exception = Assert.Throws<NoListenerFoundException>(() => _bufferMessenger.Unsubscribe(_intListener));
            Assert.Multiple(() =>
            {
                Assert.That(exception.Listener, Is.EqualTo(_intListener));
                Assert.That(exception.ListenerType, Is.EqualTo(typeof(int)));
            });
        }
    }
}
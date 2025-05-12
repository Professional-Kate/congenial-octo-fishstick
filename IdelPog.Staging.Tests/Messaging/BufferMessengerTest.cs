using IdelPog.Staging.Assertions;
using IdelPog.Staging.Exceptions;
using IdelPog.Staging.Messaging;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using Moq;

namespace IdelPog.Staging.Tests.Messaging
{
    [TestFixture]
    public class BufferMessengerTest
    {
        private IBufferMessenger _bufferMessenger { get; set; }
        private TestListener<int> _intListener { get; set; }
        private Mock<IHandler> _handlerMock { get; set; }

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
            _intListener.ResetWasCalled();
            _intListener.ResetAmountCalled();
        }

        private void Setup()
        {
            _handlerMock = new Mock<IHandler>();
            _bufferMessenger = new BufferMessenger(new AssertNotNull(_handlerMock.Object), new AssertListenerFound(_handlerMock.Object));
            
            _handlerMock.Setup(library => library.Handle(It.IsAny<ArgumentNullException>()))
                .Throws<ArgumentNullException>();
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
        public void Positive_DispatchMessage_DispatchesMessage()
        {
            _bufferMessenger.Subscribe(_intListener);
            _bufferMessenger.DispatchMessage(_bufferData);
            
            Assert.That(_intListener.BufferData, Is.EquivalentTo(_bufferData));
        }

        [Test]
        public void Positive_DispatchMessage_DispatchesCorrectMessage()
        {
            TestListener<string> stringListener = new();
            
            _bufferMessenger.Subscribe(_intListener);
            _bufferMessenger.Subscribe(stringListener);
            
            _bufferMessenger.DispatchMessage(["Testing!"]);
            _bufferMessenger.DispatchMessage(_bufferData);
            
            Assert.Multiple(() =>
            {
                Assert.That(stringListener.WasCalled, Is.True);
                Assert.That(_intListener.WasCalled, Is.True);
                Assert.That(stringListener.AmountCalled, Is.EqualTo(1));
                Assert.That(_intListener.AmountCalled, Is.EqualTo(1));
            });
        }

        [Test]
        public void Positive_DispatchMessage_NoListener_NoThrow()
        {
            Assert.DoesNotThrow(() => _bufferMessenger.DispatchMessage(_bufferData));
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
        public void Negative_Subscribe_NullListener_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _bufferMessenger.Subscribe<int>(null!));
        }

        [Test]
        public void Negative_DispatchMessage_NullBuffer_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _bufferMessenger.DispatchMessage<int>(null!));
        }

        [Test]
        public void Negative_Unsubscribe_NullListener_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _bufferMessenger.Unsubscribe<int>(null!));
        }

        [Test]
        public void Negative_Unsubscribe_NotSubscribed_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<NoListenerFoundException>()))
                .Throws(new NoListenerFoundException(_intListener));
            
            Assert.Throws<NoListenerFoundException>(() => _bufferMessenger.Unsubscribe(_intListener));
        }
    }
}
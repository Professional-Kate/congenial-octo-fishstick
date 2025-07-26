using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using Moq;

namespace IdelPog.Messaging.Tests.Collection
{
    [TestFixture]
    public class BufferTest
    {
        private Buffer<int> _buffer { get; set; }
        private int[] _data = [];
        private IBufferAssertion _bufferAssertion { get; set; }

        private bool _readyCalled;

        [SetUp]
        public void SetUp()
        {
            _readyCalled = false;

            _bufferAssertion = new BufferAssertion(new ThrowHandler());
            _buffer = new Buffer<int>(_bufferAssertion, new ObjectNullAssertion(new ThrowHandler()), new BufferRequest(3));

            if (_buffer is IInternalBuffer internalBuffer)
            {
                internalBuffer.Ready += AssertBuffer;
            }

            _data = [1, 2, 3];
        }

        private void AssertBuffer(IInternalBuffer buffer)
        {
            _readyCalled = true;
            Assert.That(buffer, Is.Not.Null);
        }

        [Test]
        public void Positive_OnConstruct_SetsState()
        {
            Buffer<int> createdBuffer = new(_bufferAssertion, new ObjectNullAssertion(new ThrowHandler()), new BufferRequest(3));

            Assert.That(createdBuffer.State, Is.EqualTo(BufferState.CREATED));
        }

        [Test]
        public void Positive_Assign_AssignsList()
        {
            _buffer.Assign(_data);

            IReadOnlyList<int> readOnlyList = _buffer.Data;

            Assert.Multiple(() =>
            {
                Assert.That(readOnlyList, Has.Count.EqualTo(3));
                Assert.That(readOnlyList, Is.EquivalentTo(_data));
            });

            _buffer.MarkReady();
            Assert.That(_readyCalled, Is.True);
        }

        [Test]
        public void Positive_Assign_ChangesState()
        {
            _buffer.Assign(_data);

            Assert.That(_buffer.State, Is.EqualTo(BufferState.FILLED));
        }

        [Test]
        public void Positive_MarkReady_CallsEvent()
        {
            _buffer.Assign(_data);
            _buffer.MarkReady();

            Assert.That(_readyCalled, Is.True);
        }

        [Test]
        public void Positive_MarkReady_ChangesState()
        {
            _buffer.Assign(_data);
            _buffer.MarkReady();

            Assert.That(_buffer.State, Is.EqualTo(BufferState.READY));
        }

        [Test]
        public void Positive_NotMarkingReady_DoesNotCallEvent()
        {
            _buffer.Assign(_data);

            Assert.That(_readyCalled, Is.False);
        }

        [Test]
        public void Positive_Data_ReturnsReadOnlyList()
        {
            _buffer.Assign(_data);

            IReadOnlyList<int> readOnlyList = _buffer.Data;

            Assert.Multiple(() =>
            {
                Assert.That(readOnlyList, Has.Count.EqualTo(3));
                Assert.That(readOnlyList, Is.EquivalentTo(_data));
            });
        }

        [Test]
        public void Negative_Assign_Null_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _buffer.Assign(null!));
        }

        [Test]
        public void Negative_Assign_AlreadyFilledState_Throws()
        {
            _buffer.Assign(_data);

            Assert.Throws<InvalidBufferStateException>(() => _buffer.Assign(_data));
            Assert.That(_buffer.State, Is.EqualTo(BufferState.FILLED));
        }

        [Test]
        public void Negative_Assign_ReadyState_Throws()
        {
            _buffer.Assign(_data);
            _buffer.MarkReady();

            Assert.Throws<InvalidBufferStateException>(() => _buffer.Assign(_data));
            Assert.That(_buffer.State, Is.EqualTo(BufferState.READY));
        }

        [TestCase(4)]
        [TestCase(2)]
        [TestCase(0)]
        public void Negative_Assign_DifferentLengthArray_Throws(int size)
        {
            int[] numbers = Enumerable.Range(0, size).ToArray();

            Assert.Throws<BufferSizeMismatchException>(() => _buffer.Assign(numbers));
        }

        [Test]
        public void Negative_MarkReady_NotFilledState_Throws()
        {
            Assert.Throws<InvalidBufferStateException>(() => _buffer.MarkReady());
            Assert.That(_buffer.State, Is.EqualTo(BufferState.CREATED));
        }

        [Test]
        public void Negative_MarkReady_Twice_Throws()
        {
            _buffer.Assign(_data);
            _buffer.MarkReady();

            Assert.Throws<InvalidBufferStateException>(() => _buffer.MarkReady());
            Assert.That(_buffer.State, Is.EqualTo(BufferState.READY));
        }
    }
}
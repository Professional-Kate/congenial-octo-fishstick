using IdelPog.Staging.Assertions.Pipelines;
using IdelPog.Staging.Collection;
using IdelPog.Staging.Exceptions;
using Moq;

namespace IdelPog.Staging.Tests.Collection
{
    [TestFixture]
    public class BufferTest
    {
        private Buffer<int> _buffer { get; set; }
        private int[] _data = [];
        private Mock<IBufferAsserter> _bufferAsserterMock { get; set; }

        private bool _readyCalled;

        [SetUp]
        public void SetUp()
        {
            _readyCalled = false;
            
            _bufferAsserterMock = new Mock<IBufferAsserter>();
            _buffer = new Buffer<int>(_bufferAsserterMock.Object, new BufferRequest<int>(3));
            
            
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
        public void Positive_MarkReady_CallsEvent()
        {
            _buffer.MarkReady();
            
            Assert.That(_readyCalled, Is.True);
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
            _bufferAsserterMock.Setup(library => library.CollectionAsserter(3, It.IsAny<ICollection<int>>()))
                .Throws(new ArgumentNullException());
            
            Assert.Throws<ArgumentNullException>(() => _buffer.Assign(null!));
        }

        [TestCase(4)]
        [TestCase(2)]
        [TestCase(0)]
        public void Negative_Assign_DifferentLengthArray_Throws(int size)
        {
            int[] numbers = Enumerable.Range(0, size).ToArray();
            
            _bufferAsserterMock.Setup(library => library.CollectionAsserter(3, numbers))
                .Throws(new BufferSizeMismatchException(3, size));
            
            Assert.Throws<BufferSizeMismatchException>(() => _buffer.Assign(numbers));
        }
    }
}
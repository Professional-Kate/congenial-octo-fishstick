using IdelPog.Staging.Collection;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers;
using Moq;

namespace IdelPog.Staging.Tests.Collection
{
    [TestFixture]
    public class BufferTest
    {
        private Buffer<int> _buffer { get; set; }
        private int[] _data = [];
        private Mock<IHandler> _handlerMock { get; set; }

        private bool _readyCalled;

        [SetUp]
        public void SetUp()
        {
            _readyCalled = false;
            
            _handlerMock = new Mock<IHandler>();
            _buffer = new Buffer<int>(new AssertNotNull(_handlerMock.Object), new BufferRequest<int>(3));
            
            _buffer.Ready += AssertBuffer;
            _data = [1, 2, 3]; 
        }

        private void AssertBuffer(IBuffer buffer)
        {
            _readyCalled = true;
            Assert.That(buffer, Is.Not.Null);
        }

        private static IEnumerable<int> StreamData(int length)
        {
            return Enumerable.Range(1, length);
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
        public void Positive_StreamInto_PopulatesData()
        {
            _buffer.StreamInto(StreamData(3));
            IReadOnlyList<int> readOnlyList = _buffer.Data;
            
            Assert.Multiple(() =>
            {
                Assert.That(readOnlyList, Has.Count.EqualTo(3));
                Assert.That(readOnlyList, Is.EquivalentTo(_data));
            });
        }

        [Test]
        public void Negative_StreamInto_PassedNull_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => _buffer.StreamInto(null!));
        }

        [Test]
        public void Negative_Assign_Null_Throws()
        {
            _handlerMock.Setup(library => library.Handle(It.IsAny<ArgumentNullException>()))
                .Throws(new ArgumentNullException());
            
            Assert.Throws<ArgumentNullException>(() => _buffer.Assign(null!));
        }

        [TestCase(4)]
        [TestCase(2)]
        [TestCase(0)]
        public void Negative_Assign_DifferentLengthArray_Throws(int size)
        {
            int[] numbers = Enumerable.Range(0, size).ToArray();
            
            Assert.Throws<Exception>(() => _buffer.Assign(numbers));
        }
        
        [TestCase(4)]
        [TestCase(2)]
        [TestCase(0)]
        public void Negative_StreamInto_DifferentLengthArray_Throws(int size)
        {
            Assert.Throws<Exception>(() => _buffer.StreamInto(StreamData(size)));
        }
    }
}
using IdelPog.Staging.Collection;

namespace IdelPog.Staging.Tests.Collection
{
    [TestFixture]
    public class BufferTest
    {
        private Buffer<int> _buffer { get; set; }
        private List<int> _data = [];

        private bool _readyCalled;

        [SetUp]
        public void SetUp()
        {
            _readyCalled = false;
            
            _buffer = new Buffer<int>();
            _buffer.Ready += AssertBuffer;
            _data = [1, 2, 3]; 
        }

        private void AssertBuffer(IBuffer buffer)
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
            Assert.Throws<ArgumentNullException>(() => _buffer.Assign(null!));
        }

      
    }
}
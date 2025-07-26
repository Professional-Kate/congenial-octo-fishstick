using IdelPog.Messaging.Assertions;
using IdelPog.Messaging.Buffer;
using IdelPog.Messaging.Exceptions;
using IdelPog.Validation.Assertions.Handlers;

namespace IdelPog.Messaging.Tests.Assertions
{
    [TestFixture]
    public class BufferAssertionTest
    {
        private IBufferAssertion _bufferAssertion { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _bufferAssertion = new BufferAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertStateEquals_SameState_NoThrow()
        {
            Assert.DoesNotThrow(() => _bufferAssertion.AssertStateEquals(BufferState.CREATED, BufferState.CREATED));
        }

        [Test]
        public void Negative_AssertStateEquals_DifferentState_Throws()
        {
            Assert.Throws<InvalidBufferStateException>(() => _bufferAssertion.AssertStateEquals(BufferState.FILLED, BufferState.CREATED));
        }
        
        [Test]
        public void Positive_AssertSizeIsValid_ValidSize_NoThrow()
        {
            Assert.DoesNotThrow(() => _bufferAssertion.AssertSizeIsValid(5));
        }

        [TestCase(0)]
        [TestCase(-1)]
        public void Negative_AssertSizeIsValid_InvalidSize_Throws(int amount)
        {
            Assert.Throws<InvalidBufferStateException>(() => _bufferAssertion.AssertSizeIsValid(amount));
        }
        
        [Test]
        public void Positive_AssertCountEquals_CountEquals_NoThrow()
        {
            Assert.DoesNotThrow(() => _bufferAssertion.AssertCountEquals(3, 3));
        }

        [Test]
        public void Negative_AssertCountEquals_CountDoesNotEqual_Throws()
        {
            Assert.Throws<InvalidBufferStateException>(() => _bufferAssertion.AssertCountEquals(1, 2));
        }
    }
}
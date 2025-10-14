using IdelPog.Core.Messaging.Assertion;
using IdelPog.Core.Messaging.Assertion.Interface;
using IdelPog.Core.Messaging.Buffer;
using IdelPog.Core.Messaging.Exceptions;

namespace IdelPog.Core.Tests.Messaging.Assertion
{
    [TestFixture]
    public sealed class BufferAssertionTest
    {
        private IBufferAssertion _bufferAssertion { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _bufferAssertion = new BufferAssertion();
        }

        [Test]
        public void Positive_AssertStateEquals_SameState_NoThrow()
        {
            Assert.DoesNotThrow(() => _bufferAssertion.AssertStateEquals(BufferState.CREATED, BufferState.CREATED));
        }

        [Test]
        public void Negative_AssertStateEquals_DifferentState_Throws()
        {
            InvalidBufferStateException exception =
                Assert.Throws<InvalidBufferStateException>(() => _bufferAssertion.AssertStateEquals(BufferState.FILLED, BufferState.CREATED));

            Assert.Multiple(() =>
            {
                Assert.That(exception.Actual, Is.EqualTo(BufferState.FILLED));
                Assert.That(exception.Expected, Is.EqualTo(BufferState.CREATED));
            });
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
            BufferSizeInvalidException exception = Assert.Throws<BufferSizeInvalidException>(() => _bufferAssertion.AssertSizeIsValid(amount));
            Assert.That(exception.Size, Is.EqualTo(amount));
        }

        [Test]
        public void Positive_AssertCountEquals_CountEquals_NoThrow()
        {
            Assert.DoesNotThrow(() => _bufferAssertion.AssertCountEquals(3, 3));
        }

        [Test]
        public void Negative_AssertCountEquals_CountDoesNotEqual_Throws()
        {
            BufferSizeMismatchException exception = Assert.Throws<BufferSizeMismatchException>(() => _bufferAssertion.AssertCountEquals(1, 2));
            Assert.Multiple(() =>
            {
                Assert.That(exception.ActualSize, Is.EqualTo(1));
                Assert.That(exception.ExpectedSize, Is.EqualTo(2));
            });
        }
    }
}
using IdelPog.Progression.Assertion;
using IdelPog.Progression.Exceptions;

namespace IdelPog.Progression.Tests.Assertion
{
    [TestFixture]
    public sealed class QueueAssertionTest
    {
        private QueueAssertion _queueAssertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _queueAssertion = new QueueAssertion();
        }

        [Test]
        public void Positive_AssertSuccessfulDequeue_SuccessfulDequeue_NoThrow()
        {
            Assert.DoesNotThrow(() => _queueAssertion.AssertSuccessfulDequeue(true));
        }

        [Test]
        public void Negative_AssertSuccessfulDequeue_UnsuccessfulDequeue_Throws()
        {
            Assert.Throws<UnsuccessfulDequeueException>(() => _queueAssertion.AssertSuccessfulDequeue(false));
        }
        
        [Test]
        public void Positive_AssertSuccessfulPeek_SuccessfulPeek_NoThrow()
        {
            Assert.DoesNotThrow(() => _queueAssertion.AssertSuccessfulPeek(true));
        }

        [Test]
        public void Negative_AssertSuccessfulPeek_UnsuccessfulPeek_Throws()
        {
            Assert.Throws<UnsuccessfulPeekException>(() => _queueAssertion.AssertSuccessfulPeek(false));
        }
    }
}
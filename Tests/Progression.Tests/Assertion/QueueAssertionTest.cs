using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Handler;
using IdelPog.Progression.Assertion;
using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Contracts;
using IdelPog.Progression.Exceptions;
using IdelPog.Progression.Runtime.ECS.Component;

namespace IdelPog.Progression.Tests.Assertion
{
    [TestFixture]
    public sealed class QueueAssertionTest
    {
        private IQueueAssertion<HarvestNodeUnlockResponse> _queueAssertion;
        private NodeLevelRequirement<HarvestNodeUnlockResponse> _nodeLevelRequirement;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _queueAssertion = new QueueAssertion<HarvestNodeUnlockResponse>(new ThrowHandler());
            
            _nodeLevelRequirement = new NodeLevelRequirement<HarvestNodeUnlockResponse>
            {
                Level = 1, SkillID = SkillID.FORAGING, OnUnlockCommand = new HarvestNodeUnlockResponse { ItemID = ItemID.BIRCH, SkillID = SkillID.FORAGING, SkillLevel = 1 }
            };
        }

        [Test]
        public void Positive_AssertSuccessfulDequeue_SuccessfulDequeue_NoThrow()
        {
            Assert.DoesNotThrow(() => _queueAssertion.AssertSuccessfulDequeue(true, _nodeLevelRequirement));
        }

        [Test]
        public void Negative_AssertSuccessfulDequeue_UnsuccessfulDequeue_Throws()
        {
            Assert.Throws<UnsuccessfulDequeueException<HarvestNodeUnlockResponse>>(() => _queueAssertion.AssertSuccessfulDequeue(false, _nodeLevelRequirement));
        }
    }
}
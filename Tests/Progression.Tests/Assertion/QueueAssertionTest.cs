using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;
using IdelPog.Core.Validation.Handler;
using IdelPog.Progression.Assertion;
using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Exceptions;
using IdelPog.Progression.Runtime.Component;

namespace IdelPog.Progression.Tests.Assertion
{
    [TestFixture]
    public sealed class QueueAssertionTest
    {
        private IQueueAssertion<SkillID, HarvestNodeUnlockResponse> _queueAssertion;
        private LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse> _levelRequirementComponent;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _queueAssertion = new QueueAssertion<SkillID, HarvestNodeUnlockResponse>(new ThrowHandler());
            
            _levelRequirementComponent = new LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse>
            {
                Level = 1, ID = SkillID.FORAGING, OnUnlockCommand = new HarvestNodeUnlockResponse { ItemID = ItemID.BIRCH, SkillID = SkillID.FORAGING }
            };
        }

        [Test]
        public void Positive_AssertSuccessfulDequeue_SuccessfulDequeue_NoThrow()
        {
            Assert.DoesNotThrow(() => _queueAssertion.AssertSuccessfulDequeue(true, _levelRequirementComponent));
        }

        [Test]
        public void Negative_AssertSuccessfulDequeue_UnsuccessfulDequeue_Throws()
        {
            Assert.Throws<UnsuccessfulDequeueException<SkillID, HarvestNodeUnlockResponse>>(() => _queueAssertion.AssertSuccessfulDequeue(false, _levelRequirementComponent));
        }
    }
}
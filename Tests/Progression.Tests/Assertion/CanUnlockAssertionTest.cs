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
    public sealed class CanUnlockAssertionTest
    {
        private ICanUnlockAssertion<HarvestNodeUnlockResponse> _canUnlockAssertion;
        private NodeLevelRequirement<HarvestNodeUnlockResponse> _nodeLevelRequirement;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _canUnlockAssertion = new CanUnlockAssertion<HarvestNodeUnlockResponse>(new ThrowHandler());

            _nodeLevelRequirement = new NodeLevelRequirement<HarvestNodeUnlockResponse>()
            {
                Level = 1, SkillID = SkillID.FORAGING, OnUnlockCommand = new HarvestNodeUnlockResponse { ItemID = ItemID.BIRCH, SkillID = SkillID.FORAGING, SkillLevel = 1 }
            };
        }

        [Test]
        public void Positive_AssertCanUnlock_CanUnlock_NoThrow()
        {
            Assert.DoesNotThrow(() => _canUnlockAssertion.AssertCanUnlock(1, 1, _nodeLevelRequirement));
        }

        [Test]
        public void Negative_AssertCanUnlock_NoUnlock_Throws()
        {
            Assert.Throws<CannotUnlockException<HarvestNodeUnlockResponse>>(() => _canUnlockAssertion.AssertCanUnlock(1, 5, _nodeLevelRequirement));
        }
    }
}
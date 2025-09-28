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
    public sealed class CanUnlockAssertionTest
    {
        private ICanUnlockAssertion<SkillID, HarvestNodeUnlockResponse> _canUnlockAssertion;
        private LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse> _levelRequirementComponent;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _canUnlockAssertion = new CanUnlockAssertion<SkillID, HarvestNodeUnlockResponse>(new ThrowHandler());

            _levelRequirementComponent = new LevelRequirementComponent<SkillID, HarvestNodeUnlockResponse>
            {
                Level = 1, ID = SkillID.FORAGING, OnUnlockCommand = new HarvestNodeUnlockResponse { ItemID = ItemID.BIRCH, SkillID = SkillID.FORAGING, SkillLevel = 1 }
            };
        }

        [Test]
        public void Positive_AssertCanUnlock_CanUnlock_NoThrow()
        {
            Assert.DoesNotThrow(() => _canUnlockAssertion.AssertCanUnlock(1, 1, _levelRequirementComponent));
        }

        [Test]
        public void Negative_AssertCanUnlock_NoUnlock_Throws()
        {
            Assert.Throws<CannotUnlockException<SkillID, HarvestNodeUnlockResponse>>(() => _canUnlockAssertion.AssertCanUnlock(1, 5, _levelRequirementComponent));
        }
    }
}
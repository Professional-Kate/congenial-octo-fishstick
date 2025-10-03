using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Handler;
using IdelPog.Progression.Assertion;
using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Exceptions;

namespace IdelPog.Progression.Tests.Assertion
{
    [TestFixture]
    public sealed class SkillMatchesAssertionTest
    {
        private ISkillMatchesAssertion<SkillID> _skillMatchesAssertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _skillMatchesAssertion = new SkillMatchesAssertion<SkillID>(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertSkillMatches_SkillMatches_NoThrow()
        {
            Assert.DoesNotThrow(() => _skillMatchesAssertion.AssertSkillMatches(SkillID.FORAGING, SkillID.FORAGING));
        }

        [Test]
        public void Negative_AssertSkillMatches_DifferentSkills_Throws()
        {
            Assert.Throws<IDMismatchException<SkillID>>(() => _skillMatchesAssertion.AssertSkillMatches(SkillID.FORAGING, SkillID.MINING));
        }
    }
}
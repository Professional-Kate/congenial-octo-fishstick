using IdelPog.Core.Contracts.Enum;
using IdelPog.Progression.Assertion;
using IdelPog.Progression.Assertion.Interface;
using IdelPog.Progression.Exceptions;

namespace IdelPog.Progression.Tests.Assertion
{
    [TestFixture]
    public sealed class IDMatchesAssertionTest
    {
        private IIDMatchesAssertion<SkillID> _iidMatchesAssertion;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _iidMatchesAssertion = new IDMatchesAssertion<SkillID>();
        }

        [Test]
        public void Positive_AssertSkillMatches_SkillMatches_NoThrow()
        {
            Assert.DoesNotThrow(() => _iidMatchesAssertion.AssertIDMatches(SkillID.FORAGING, SkillID.FORAGING));
        }

        [Test]
        public void Negative_AssertSkillMatches_DifferentSkills_Throws()
        {
            Assert.Throws<IDMismatchException<SkillID>>(() => _iidMatchesAssertion.AssertIDMatches(SkillID.FORAGING, SkillID.MINING));
        }
    }
}
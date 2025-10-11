using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Handler;
using IdelPog.HarvestNode.Assertion;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Exceptions;

namespace IdelPog.HarvestNode.Tests.Assertion
{
    [TestFixture]
    public sealed class NodeUnlockedAssertionTest
    {
        private NodeUnlockedAssertion _nodeUnlockedAssertion;
        private HarvestNodeUpdate _harvestNodeUpdate;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            _harvestNodeUpdate = new HarvestNodeUpdate { SkillID = SkillID.MINING, ResourceID = ResourceID.ANT_NEST };
            _nodeUnlockedAssertion = new NodeUnlockedAssertion(new ThrowHandler());
        }

        [Test]
        public void Positive_AssertNodeIsUnlocked_PassedTrue_NoThrow()
        {
            Assert.DoesNotThrow(() => _nodeUnlockedAssertion.AssertNodeIsUnlocked(true, _harvestNodeUpdate));
        }

        [Test]
        public void Negative_AssertNodeIsUnlocked_PassedFalse_Throws()
        {
            Assert.Throws<HarvestNodeLockedException>(() => _nodeUnlockedAssertion.AssertNodeIsUnlocked(false, _harvestNodeUpdate));
        }
    }
}
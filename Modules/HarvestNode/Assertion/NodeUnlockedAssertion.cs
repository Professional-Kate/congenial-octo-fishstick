using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.HarvestNode.Assertion.Interface;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Exceptions;

namespace IdelPog.HarvestNode.Assertion
{
    public sealed class NodeUnlockedAssertion : BaseAssertion, INodeUnlockedAssertion
    {
        public NodeUnlockedAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertNodeIsUnlocked(bool unlocked, HarvestNodeUpdate update)
        {
            Assert<HarvestNodeLockedException>(() =>
            {
                if (unlocked == false)
                {
                    throw new HarvestNodeLockedException(update.SkillID, update.ResourceID);
                }
            });
        }
    }
}
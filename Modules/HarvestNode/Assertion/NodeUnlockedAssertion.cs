using IdelPog.HarvestNode.Assertion.Interface;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Exceptions;

namespace IdelPog.HarvestNode.Assertion
{
    public sealed class NodeUnlockedAssertion : INodeUnlockedAssertion
    {
        public void AssertNodeIsUnlocked(bool unlocked, HarvestNodeUpdate update)
        {
            if (unlocked == false)
            {
                throw new HarvestNodeLockedException(update.SkillID, update.ResourceID);
            }
        }
    }
}
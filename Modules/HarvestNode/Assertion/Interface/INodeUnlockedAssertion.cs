using IdelPog.Core.Contracts.Command;

namespace IdelPog.HarvestNode.Assertion.Interface
{
    public interface INodeUnlockedAssertion
    {
        public void AssertNodeIsUnlocked(bool unlocked, HarvestNodeUpdate update);
    }
}
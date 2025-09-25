using IdelPog.Progression.Contracts;
using IdelPog.Progression.Runtime.ECS.System.Interface;

namespace IdelPog.Progression.Runtime.ECS.System
{
    public class NodeUnlockerService : INodeUnlockerService
    {
        public bool CanUnlock(HarvestNodeUnlock harvestNodeUnlock)
        {
            throw new NotImplementedException();
        }

        public HarvestNodeUnlockResponse Unlock(HarvestNodeUnlock harvestNodeUnlock)
        {
            throw new NotImplementedException();
        }
    }
}
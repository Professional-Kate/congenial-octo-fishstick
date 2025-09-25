using IdelPog.Progression.Contracts;

namespace IdelPog.Progression.Runtime.ECS.System.Interface
{
    public interface INodeUnlockerService
    {
        public bool CanUnlock(HarvestNodeUnlock harvestNodeUnlock);
        
        public HarvestNodeUnlockResponse Unlock(HarvestNodeUnlock harvestNodeUnlock);
    }
}
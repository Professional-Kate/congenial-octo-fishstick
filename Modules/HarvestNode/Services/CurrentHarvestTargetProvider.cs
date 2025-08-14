using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Services
{
    public class CurrentHarvestTargetProvider : ICurrentHarvestTargetProvider, ICurrentHarvestTargetSetter
    {
        private ItemID _harvestTarget;

        public ItemID GetCurrentHarvestTarget()
        {
            return _harvestTarget;
        }

        public void SetCurrentResource(ItemID harvestTarget)
        {
            _harvestTarget = harvestTarget;
        }
    }
}
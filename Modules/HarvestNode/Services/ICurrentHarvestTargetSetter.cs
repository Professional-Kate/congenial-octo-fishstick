using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Services
{
    public interface ICurrentHarvestTargetSetter
    {
        public void SetCurrentResource(ItemID harvestTarget);
    }
}
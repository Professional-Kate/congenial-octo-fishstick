using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Services
{
    public interface ICurrentHarvestTargetProvider
    {
        public ItemID GetCurrentHarvestTarget();
    }
}
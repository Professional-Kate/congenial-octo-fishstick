using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Services
{
    public interface ICurrentResourceSetter
    {
        public void SetCurrentResource(ResourceID resource);
    }
}
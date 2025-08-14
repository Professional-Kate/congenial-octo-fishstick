using IdelPog.Core.Contracts.Enum;

namespace IdelPog.HarvestNode.Services
{
    public interface ICurrentResourceProvider
    {
        public ResourceID GetCurrentResource();
    }
}
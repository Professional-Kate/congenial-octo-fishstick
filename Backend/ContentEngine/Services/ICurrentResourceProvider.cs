using IdelPog.Common.Enums;

namespace ContentEngine.Services
{
    public interface ICurrentResourceProvider
    {
        public ResourceID GetCurrentResource();
    }
}
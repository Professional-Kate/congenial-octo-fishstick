using IdelPog.Common.Enums;

namespace ContentEngine.Services
{
    public interface ICurrentResourceSetter
    {
        public void SetCurrentResource(ResourceID resource);
    }
}
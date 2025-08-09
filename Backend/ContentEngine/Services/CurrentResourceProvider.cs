using IdelPog.Common.Enums;

namespace ContentEngine.Services
{
    public class CurrentResourceProvider : ICurrentResourceProvider, ICurrentResourceSetter
    {
        private ResourceID _currentResource;

        public ResourceID GetCurrentResource()
        {
            return _currentResource;
        }

        public void SetCurrentResource(ResourceID resource)
        {
            _currentResource = resource;
        }
    }
}
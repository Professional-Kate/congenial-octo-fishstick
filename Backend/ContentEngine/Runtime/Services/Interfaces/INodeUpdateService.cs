using IdelPog.Common.Enums;
using IdelPog.Common.Responses;

namespace ContentEngine.Runtime.Services
{
    public interface INodeUpdateService
    {
        public HarvestNodeUpdateResponse UpdateHarvestNode(ResourceID resourceID);
    }
}
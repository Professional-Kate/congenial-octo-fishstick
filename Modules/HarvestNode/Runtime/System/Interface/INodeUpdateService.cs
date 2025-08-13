using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Contracts.Response;

namespace IdelPog.HarvestNode.Runtime.System.Interface
{
    public interface INodeUpdateService
    {
        public HarvestNodeUpdateResponse UpdateHarvestNode(ResourceID resourceID);
    }
}
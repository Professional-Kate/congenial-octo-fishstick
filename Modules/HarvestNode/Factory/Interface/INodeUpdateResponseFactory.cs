using IdelPog.Core.Contracts.Response;

namespace IdelPog.HarvestNode.Factory.Interface
{
    public interface INodeUpdateResponseFactory
    {
        public HarvestNodeUpdateResponse Create(Contracts.HarvestNode harvestNode, bool hasLeveled);
    }
}
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;
using IdelPog.HarvestNode.Factory.Interface;

namespace IdelPog.HarvestNode.Factory
{
    public class NodeCreationResponseFactory : INodeCreationResponseFactory
    {
        public HarvestNodeCreationResponse Create(HarvestNodeCreation[] nodeCreations)
        {
            return new HarvestNodeCreationResponse
            {
                NodeCreations = nodeCreations
            };
        }
    }
}
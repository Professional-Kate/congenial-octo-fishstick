using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;

namespace IdelPog.HarvestNode.Factory.Interface
{
    public interface INodeCreationResponseFactory
    {
        public HarvestNodeCreationResponse Create(HarvestNodeCreation[] nodeCreations);
    }
}
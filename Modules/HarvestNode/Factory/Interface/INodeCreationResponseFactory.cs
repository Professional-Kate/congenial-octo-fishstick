using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Response;

namespace IdelPog.HarvestNode.Factory.Interface
{
    public interface INodeCreationResponseFactory
    {
        public IReadOnlyList<HarvestNodeCreationResponse> Create(HarvestNodeCreation[] nodeCreations);
    }
}
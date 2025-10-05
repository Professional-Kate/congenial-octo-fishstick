using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;
using IdelPog.HarvestNode.Factory.Interface;

namespace IdelPog.HarvestNode.Factory
{
    public sealed class NodeCreationResponseFactory : INodeCreationResponseFactory
    {
        public IReadOnlyList<HarvestNodeCreationResponse> Create(HarvestNodeCreation[] nodeCreations)
        {
            HarvestNodeCreationResponse[] responses = new HarvestNodeCreationResponse[nodeCreations.Length];
            for (var i = 0; i < nodeCreations.Length; i++)
            {
                HarvestNodeCreation creation = nodeCreations[i];
                HarvestNodeCreationResponse response = new HarvestNodeCreationResponse { LinkedSkill = creation.LinkedSkill, ReadOnlyHarvestNodes = creation.ReadOnlyHarvestNodes};
                responses[i] = response;
            }
            
            return responses;
        }
    }
}
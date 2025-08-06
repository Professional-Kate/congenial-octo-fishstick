using IdelPog.Common.Commands;
using IdelPog.Common.Responses;

namespace IdelPog.Common.Factories
{
    public class NodeCreationResponseFactory : INodeCreationResponseFactory
    {
        public NodeCreationResponse Create(NodeCreation[] nodeCreations)
        {
            return new NodeCreationResponse
            {
                NodeCreations = nodeCreations,
            };
        }
    }
}
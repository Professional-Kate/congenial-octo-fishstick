using IdelPog.Common.Commands;
using IdelPog.Common.Responses;

namespace IdelPog.Common.Factories
{
    public interface INodeCreationResponseFactory
    {
        public NodeCreationResponse Create(NodeCreation[] nodeCreations);
    }
}
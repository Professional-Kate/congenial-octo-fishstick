using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.HarvestNode.Factory
{
    public class NodeCreationErrorFactory : IErrorFactory<NodeCreationError, IReadOnlyList<NodeCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public NodeCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public NodeCreationError Create<TException>(TException exception, IReadOnlyList<NodeCreation> context) where TException : Exception
        {
            return new NodeCreationError
            {
                NodeCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
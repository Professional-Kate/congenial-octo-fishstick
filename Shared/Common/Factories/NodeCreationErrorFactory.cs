using IdelPog.Common.Commands;
using IdelPog.Common.Errors;

namespace IdelPog.Common.Factories
{
    public class NodeCreationErrorFactory : IErrorFactory<NodeCreationError, IReadOnlyList<NodeCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public NodeCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public NodeCreationError Create<TException>(IReadOnlyList<NodeCreation> context, TException exception) where TException : Exception
        {
            return new NodeCreationError
            {
                NodeCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
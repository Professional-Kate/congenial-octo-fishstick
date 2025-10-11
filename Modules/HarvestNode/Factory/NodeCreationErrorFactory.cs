using IdelPog.Core.Factory.Interface;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Error;

namespace IdelPog.HarvestNode.Factory
{
    public class NodeCreationErrorFactory : IErrorFactory<HarvestNodeCreationError, IReadOnlyList<HarvestNodeCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public NodeCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public HarvestNodeCreationError Create<TException>(TException exception, IReadOnlyList<HarvestNodeCreation> context) where TException : Exception
        {
            return new HarvestNodeCreationError
            {
                NodeCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
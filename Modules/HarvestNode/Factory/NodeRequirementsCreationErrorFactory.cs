using IdelPog.Core.Factory.Interface;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Error;

namespace IdelPog.HarvestNode.Factory
{
    public sealed class NodeRequirementsCreationErrorFactory : IErrorFactory<HarvestNodeRequirementsCreationError, IReadOnlyList<HarvestNodeRequirementsCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public NodeRequirementsCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public HarvestNodeRequirementsCreationError Create<TException>(TException exception, IReadOnlyList<HarvestNodeRequirementsCreation> context) where TException : Exception
        {
            return new HarvestNodeRequirementsCreationError
            {
                HarvestNodeRequirementsCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;

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
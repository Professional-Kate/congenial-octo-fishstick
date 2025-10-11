using IdelPog.Core.Factory.Interface;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Error;

namespace IdelPog.HarvestNode.Factory
{
    public sealed class NodeLootCreationErrorFactory : IErrorFactory<ResourceLootCreationError, IReadOnlyList<ResourceLootCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public NodeLootCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public ResourceLootCreationError Create<TException>(TException exception, IReadOnlyList<ResourceLootCreation> context) where TException : Exception
        {
            return new ResourceLootCreationError
            {
                HarvestNodeLootCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
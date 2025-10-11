using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;

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
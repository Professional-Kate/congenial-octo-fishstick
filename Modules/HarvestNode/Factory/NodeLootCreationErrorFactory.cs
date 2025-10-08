using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.HarvestNode.Factory
{
    public sealed class NodeLootCreationErrorFactory : IErrorFactory<HarvestNodeLootCreationError, IReadOnlyList<HarvestNodeLootCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public NodeLootCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public HarvestNodeLootCreationError Create<TException>(TException exception, IReadOnlyList<HarvestNodeLootCreation> context) where TException : Exception
        {
            return new HarvestNodeLootCreationError
            {
                HarvestNodeLootCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
using IdelPog.Core.Factory.Interface;
using IdelPog.HarvestNode.Contracts.Command;
using IdelPog.HarvestNode.Contracts.Error;

namespace IdelPog.HarvestNode.Factory
{
    public sealed class LocationLootCreationErrorFactory : IErrorFactory<LocationLootCreationError, IReadOnlyList<LocationLootCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public LocationLootCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public LocationLootCreationError Create<TException>(TException exception, IReadOnlyList<LocationLootCreation> context) where TException : Exception
        {
            return new LocationLootCreationError
            {
                LocationLootCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
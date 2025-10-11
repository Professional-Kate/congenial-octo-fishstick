using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;

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
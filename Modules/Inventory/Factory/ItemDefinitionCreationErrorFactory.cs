using IdelPog.Core.Factory.Interface;
using IdelPog.Inventory.Contracts.Command;
using IdelPog.Inventory.Contracts.Error;

namespace IdelPog.Inventory.Factory
{
    public sealed class ItemDefinitionCreationErrorFactory : IErrorFactory<ItemDefinitionCreationError, IReadOnlyList<ItemDefinitionCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public ItemDefinitionCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public ItemDefinitionCreationError Create<TException>(TException exception, IReadOnlyList<ItemDefinitionCreation> context) where TException : Exception
        {
            return new ItemDefinitionCreationError
            {
                ItemDefinitionCreations = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
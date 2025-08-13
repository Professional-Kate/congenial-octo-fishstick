using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Error;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Inventory.Factory
{
    public class InventoryUpdateErrorFactory : IErrorFactory<InventoryUpdateError, IReadOnlyList<InventoryUpdate>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public InventoryUpdateErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public InventoryUpdateError Create<TException>(TException exception, IReadOnlyList<InventoryUpdate> context) where TException : Exception
        {
            return new InventoryUpdateError
            {
                BaseError = _baseErrorFactory.Create(exception),
                InventoryUpdates = context.ToArray()
            };
        }
    }
}
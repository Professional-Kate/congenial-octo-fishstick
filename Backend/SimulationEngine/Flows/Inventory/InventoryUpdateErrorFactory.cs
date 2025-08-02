using IdelPog.Common.Factories;

namespace IdelPog.SimulationEngine.Inventory
{
    public class InventoryUpdateErrorFactory : IErrorFactory<InventoryUpdateError, IReadOnlyList<InventoryUpdate>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public InventoryUpdateErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public InventoryUpdateError Create<TException>(IReadOnlyList<InventoryUpdate> context, TException exception) where TException : Exception
        {
            return new InventoryUpdateError
            {
                BaseError = _baseErrorFactory.Create(exception),
                InventoryUpdates = context.ToArray()
            };
        }
    }
}
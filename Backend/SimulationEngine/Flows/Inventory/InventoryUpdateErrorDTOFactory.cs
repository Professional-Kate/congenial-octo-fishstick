using IdelPog.Common.Factories;

namespace IdelPog.SimulationEngine.Inventory
{
    public class InventoryUpdateErrorDTOFactory : IErrorFactory<InventoryUpdateErrorDTO, IReadOnlyList<InventoryUpdate>>
    {
        private readonly IErrorDTOFactory _errorDTOFactory;

        public InventoryUpdateErrorDTOFactory(IErrorDTOFactory errorDTOFactory)
        {
            _errorDTOFactory = errorDTOFactory;
        }

        public InventoryUpdateErrorDTO Create<TException>(IReadOnlyList<InventoryUpdate> context, TException exception) where TException : Exception
        {
            return new InventoryUpdateErrorDTO
            {
                ErrorDTO = _errorDTOFactory.Create(exception),
                InventoryUpdates = context.ToArray()
            };
        }
    }
}
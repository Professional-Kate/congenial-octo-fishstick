using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public interface ICurrencyCreationErrorDTOFactory
    {
        public CurrencyCreationErrorDTO CreateCurrencyCreationError(IReadOnlyList<CurrencyCreation> currencyCreations, Exception exception);
    }
}
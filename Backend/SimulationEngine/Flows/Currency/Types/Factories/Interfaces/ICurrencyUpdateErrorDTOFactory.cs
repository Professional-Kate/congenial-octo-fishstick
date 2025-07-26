using IdelPog.Common.Commands;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public interface ICurrencyUpdateErrorDTOFactory
    {
        public CurrencyUpdateErrorDTO CreateCurrencyUpdateError(IReadOnlyList<CurrencyUpdate> updates, Exception exception);
    }
}
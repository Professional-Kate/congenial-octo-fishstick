using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public interface ICurrencyCreationErrorFactory
    {
        public void CreateCurrencyCreationError(IReadOnlyList<CurrencyCreation> currencyCreations, Exception exception);
    }
}
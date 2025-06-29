using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Dispatchers
{
    public interface ICurrencyCreationErrorDispatcher
    {
        public void Dispatch(IReadOnlyList<CurrencyCreation> currencyCreations, Exception exception);
    }
}
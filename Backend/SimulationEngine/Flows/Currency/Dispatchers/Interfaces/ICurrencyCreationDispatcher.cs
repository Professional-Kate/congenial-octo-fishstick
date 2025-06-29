using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Dispatchers
{
    public interface ICurrencyCreationDispatcher
    {
        public void Dispatch(IReadOnlyList<CurrencyCreation> createdCurrency);
    }
}
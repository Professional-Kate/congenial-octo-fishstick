using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public interface ICurrencyCreationDispatcher
    {
        public void Dispatch(IReadOnlyList<CurrencyCreation> createdCurrency);
    }
}
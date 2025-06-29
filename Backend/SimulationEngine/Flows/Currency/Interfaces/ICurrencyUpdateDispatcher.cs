using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public interface ICurrencyUpdateDispatcher
    {
        public void Dispatch(IReadOnlyList<CurrencyTrade> trades);
    }
}
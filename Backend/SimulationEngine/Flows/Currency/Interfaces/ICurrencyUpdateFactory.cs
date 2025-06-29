using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public interface ICurrencyUpdateFactory
    {
        public CurrencyUpdateDTO[] CreateFrom(IReadOnlyList<CurrencyTrade> trades);
    }
}
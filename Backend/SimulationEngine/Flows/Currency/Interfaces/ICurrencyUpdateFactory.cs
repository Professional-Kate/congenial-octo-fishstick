using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency
{
    public interface ICurrencyUpdateFactory
    {
        public CurrencyUpdateDTO[] CreateFrom(IReadOnlyList<CurrencyTrade> trades);
    }
}
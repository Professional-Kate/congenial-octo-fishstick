using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Dispatchers
{
    public interface ICurrencyUpdateSummarizer
    {
        public CurrencyUpdate[] GetSummary(IReadOnlyList<CurrencyUpdate> updates);
    }
}
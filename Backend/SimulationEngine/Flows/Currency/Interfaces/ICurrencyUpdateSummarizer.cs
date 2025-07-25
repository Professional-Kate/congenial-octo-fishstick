using IdelPog.Common.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public interface ICurrencyUpdateSummarizer
    {
        public CurrencyUpdate[] GetSummary(IReadOnlyList<CurrencyUpdate> updates);
    }
}
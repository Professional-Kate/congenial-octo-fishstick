using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public interface ICurrencyUpdateSummarizer
    {
        public CurrencyUpdate[] GetSummary(IReadOnlyList<CurrencyUpdate> updates);
    }
}
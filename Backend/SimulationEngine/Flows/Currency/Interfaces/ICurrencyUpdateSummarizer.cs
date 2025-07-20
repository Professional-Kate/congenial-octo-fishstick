using IdelPog.Common.Commands;
using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency
{
    public interface ICurrencyUpdateSummarizer
    {
        public CurrencyUpdate[] GetSummary(IReadOnlyList<CurrencyUpdate> updates);
    }
}
using IdelPog.Core.Contracts.Command;

namespace IdelPog.Currency.Service.Interface
{
    public interface ICurrencyUpdateSummarizer
    {
        public CurrencyUpdate[] GetSummary(IReadOnlyList<CurrencyUpdate> updates);
    }
}
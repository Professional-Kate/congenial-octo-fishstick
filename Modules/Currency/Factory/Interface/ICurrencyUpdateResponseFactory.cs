using IdelPog.Currency.Contracts.Response;

namespace IdelPog.Currency.Factory.Interface
{
    public interface ICurrencyUpdateResponseFactory
    {
        public IReadOnlyList<CurrencyUpdateResponse> CreateFrom(IReadOnlyList<Contracts.Currency> currencies);
    }
}
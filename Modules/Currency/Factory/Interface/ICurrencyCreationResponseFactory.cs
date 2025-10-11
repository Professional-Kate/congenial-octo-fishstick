using IdelPog.Currency.Contracts.Command;
using IdelPog.Currency.Contracts.Response;

namespace IdelPog.Currency.Factory.Interface
{
    public interface ICurrencyCreationResponseFactory
    {
        public IReadOnlyList<CurrencyCreationResponse> CreateFrom(IReadOnlyList<CurrencyCreation> currencyCreations);
    }
}
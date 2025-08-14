using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;

namespace IdelPog.Currency.Factory.Interface
{
    public interface ICurrencyCreationResponseFactory
    {
        public CurrencyCreationResponse CreateFrom(IReadOnlyList<CurrencyCreation> currencyCreations);

        public CurrencyCreationResponse CreateFrom(CurrencyCreation currencyCreation);
    }
}
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;

namespace IdelPog.Currency.Factory.Interface
{
    public interface ICurrencyUpdateResponseFactory
    {
        public CurrencyUpdateResponse CreateFrom(IReadOnlyList<CurrencyUpdate> trades);
        
        public CurrencyUpdateResponse CreateFrom(CurrencyUpdate trade);
    }
}
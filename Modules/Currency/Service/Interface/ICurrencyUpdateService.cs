using IdelPog.Core.Contracts.Command;
using IdelPog.Currency.Contracts.Response;

namespace IdelPog.Currency.Service.Interface
{
    public interface ICurrencyUpdateService
    { 
        public IReadOnlyList<CurrencyUpdateResponse> ApplyUpdates(IReadOnlyList<CurrencyUpdate> currencyUpdates);
    }
}
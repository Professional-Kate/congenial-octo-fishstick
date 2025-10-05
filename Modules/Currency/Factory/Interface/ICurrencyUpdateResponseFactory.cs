using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Response;

namespace IdelPog.Currency.Factory.Interface
{
    public interface ICurrencyUpdateResponseFactory
    {
        public IReadOnlyList<CurrencyUpdateResponse> CreateFrom(IReadOnlyList<CurrencyUpdate> trades);
    }
}
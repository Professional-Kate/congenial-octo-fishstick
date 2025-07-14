using IdelPog.Common.Enums;

namespace IdelPog.Common.Dispatchers.Interfaces
{
    public interface ICurrencyUpdateDispatcher
    {
        public void DispatchCurrencyUpdate(CurrencyUpdate currencyUpdate);
    }
}
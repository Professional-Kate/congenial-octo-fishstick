using IdelPog.Structures;

namespace IdelPog.Service.Currency
{
    public interface ICurrencyService
    {
        public ServiceResponse AddAmount(CurrencyType currencyType, int amount);
        
        public ServiceResponse RemoveAmount(CurrencyType currencyType, int amount);
    }
}
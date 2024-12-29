using IdelPog.Repository;
using IdelPog.Structures;

namespace IdelPog.Service.Currency
{
    public class CurrencyService : ICurrencyService
    {
        protected ICurrencyRepository Repository = new CurrencyRepository();
        
        public ServiceResponse AddAmount(CurrencyType currencyType, int amount)
        {
            throw new System.NotImplementedException();
        }

        public ServiceResponse RemoveAmount(CurrencyType currencyType, int amount)
        {
            throw new System.NotImplementedException();
        }
    }
}
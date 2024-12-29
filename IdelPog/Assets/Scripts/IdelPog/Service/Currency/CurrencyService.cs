using IdelPog.Exceptions;
using IdelPog.Repository;
using IdelPog.Structures;

namespace IdelPog.Service.Currency
{
    public class CurrencyService : ICurrencyService
    {
        protected ICurrencyRepository Repository = new CurrencyRepository();
        
        public ServiceResponse AddAmount(CurrencyType currencyType, int amount)
        {
            if (amount <= 0)
            {
                return ServiceResponse.Failure($"Error! Passed amount : '{amount}' is required to be a positive number.");
            }

            try
            {
                Model.Currency currency = Repository.Get(currencyType);

                int newAmount = currency.Amount + amount;
                currency.SetAmount(newAmount);
            }
            catch (NotFoundException exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }

            return ServiceResponse.Success();
        }

        public ServiceResponse RemoveAmount(CurrencyType currencyType, int amount)
        {
            throw new System.NotImplementedException();
        }
    }
}
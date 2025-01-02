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
            ServiceResponse serviceResponse = AssertAmountGreaterThanZero(amount);
            if (serviceResponse.IsSuccess == false)
            {
                return serviceResponse;
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
            ServiceResponse serviceResponse = AssertAmountGreaterThanZero(amount);
            if (serviceResponse.IsSuccess == false)
            {
                return serviceResponse;
            }

            try
            {
                Model.Currency currency = Repository.Get(currencyType);

                int newAmount = currency.Amount - amount;
                currency.SetAmount(newAmount);
            }
            catch (NotFoundException exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }

            return ServiceResponse.Success();
        }

        /// <summary>
        /// Asserts that the passed amount is greater tha n zero
        /// If the returned <see cref="ServiceResponse"/>.<see cref="ServiceResponse.IsSuccess"/> is false then it didn't pass the assertions
        /// </summary>
        /// <param name="amount">The amount you want to check</param>
        /// <returns>
        /// A <see cref="ServiceResponse"/> object that will contain a boolean <see cref="ServiceResponse.IsSuccess"/> and a possible string <see cref="ServiceResponse.Message"/>
        /// </returns>
        private static ServiceResponse AssertAmountGreaterThanZero(int amount)
        {
            if (amount <= 0)
            {
                return ServiceResponse.Failure($"Error! Passed amount : '{amount}' is required to be a positive number.");
            }
            
            return ServiceResponse.Success();
        }
    }
}
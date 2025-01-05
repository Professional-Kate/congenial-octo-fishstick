using System.Collections.Generic;
using System.Linq;
using IdelPog.Exceptions;
using IdelPog.Repository.Currency;
using IdelPog.Structures;

namespace IdelPog.Service.Currency
{
    /// <summary>
    /// See <see cref="ICurrencyService"/> for documentation.
    /// </summary>
    public class CurrencyService : ICurrencyService, ICurrencyServicePersistence
    {
        protected ICurrencyRepository Repository = new CurrencyRepository();
        
        public ServiceResponse ProcessCurrencyUpdate(params CurrencyTrade[] trades)
        {
            ServiceResponse serviceResponse = ValidateTrades(trades);
            if (serviceResponse.IsSuccess == false)
            {
                return serviceResponse;
            }
            
            Dictionary<CurrencyType, Model.Currency> stagingGround = new();
            
            ServiceResponse returnResponse = ServiceResponse.Success();

            try
            {
                foreach (CurrencyTrade currencyTrade in trades)
                {
                    Model.Currency globalCurrency = Repository.Get(currencyTrade.Currency);
                    stagingGround.TryAdd(currencyTrade.Currency, globalCurrency.Clone() as Model.Currency);

                    Model.Currency localCurrency = stagingGround[currencyTrade.Currency];

                    switch (currencyTrade.Action)
                    {
                        case ActionType.ADD:
                            int newAmount = localCurrency.Amount + currencyTrade.Amount;
                            localCurrency.SetAmount(newAmount);
                            break;
                        case ActionType.REMOVE:
                            newAmount = localCurrency.Amount - currencyTrade.Amount;
                            localCurrency.SetAmount(newAmount);
                            break;
                    }
                }
            }
            catch (NotFoundException exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }

            foreach (Model.Currency stagedCurrency in stagingGround.Select(entry => entry.Value))
            {
                serviceResponse = AssertAmountGreaterThanZero(stagedCurrency.Amount);
                if (serviceResponse.IsSuccess == false)
                {
                    return serviceResponse;
                }
            }
            
            foreach (Model.Currency stagedCurrency in stagingGround.Select(entry => entry.Value))
            {
                Model.Currency globalCurrency = Repository.Get(stagedCurrency.CurrencyType);

                int difference = stagedCurrency.Amount - globalCurrency.Amount;
                switch (difference)
                {
                    case > 0:
                        AddAmount(globalCurrency.CurrencyType, difference);
                        break;
                    case < 0:
                        RemoveAmount(globalCurrency.CurrencyType, -difference);
                        break;

                }
            }

            return returnResponse;
        }
        
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

                if (newAmount <= 0)
                {
                    return ServiceResponse.Failure($"Error! Cannot remove amount : {amount} from CurrencyType {currencyType}. This operation would cause Amount to become a negative number.");
                }
                
                currency.SetAmount(newAmount);
            }
            catch (NotFoundException exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }

            return ServiceResponse.Success();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trades"></param>
        /// <returns></returns>
        private ServiceResponse ValidateTrades(params CurrencyTrade[] trades)
        {
            foreach (CurrencyTrade currencyTrade in trades)
            {
                ServiceResponse serviceResponse = AssertAmountGreaterThanZero(currencyTrade.Amount);
                if (serviceResponse.IsSuccess == false)
                {
                    return serviceResponse;
                }
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
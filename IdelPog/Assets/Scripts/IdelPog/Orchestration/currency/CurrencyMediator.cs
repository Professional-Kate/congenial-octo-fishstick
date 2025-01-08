using System.Collections.Generic;
using System.Linq;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Repository.Currency;
using IdelPog.Service;
using IdelPog.Service.Currency;
using IdelPog.Structures;

namespace IdelPog.Orchestration.currency
{
    public class CurrencyMediator : ICurrencyMediator
    {
        private readonly ICurrencyService _currencyService;
        private readonly ICurrencyRepository _repository;

        public CurrencyMediator()
        {
            _currencyService = new CurrencyService();
            _repository = new CurrencyRepository();
        }

        public CurrencyMediator(ICurrencyService currencyService, ICurrencyRepository repository)
        {
            _currencyService = currencyService;
            _repository = repository;
        }
        
        public ServiceResponse ProcessCurrencyUpdate(params CurrencyTrade[] trades)
        {
            ServiceResponse validateTradesResponse = ValidateTrades(trades);
            if (validateTradesResponse.IsSuccess == false)
            {
                return validateTradesResponse;
            }
            
            Dictionary<CurrencyType, Currency> stagingGround = new();

            try
            {
                // Clone Repository Currency into the stagingGround
                foreach (CurrencyTrade currencyTrade in trades)
                {
                    Currency globalCurrency = _repository.Get(currencyTrade.Currency);
                    stagingGround.TryAdd(currencyTrade.Currency, globalCurrency.Clone() as Currency);

                    Currency localCurrency = stagingGround[currencyTrade.Currency];

                    // Apply CurrencyTrade actions to the stagingGround Currency
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

            // Assert if all the passed CurrencyTrades won't leave a Currency with 0 or less Amount
            foreach (Currency stagedCurrency in stagingGround.Select(entry => entry.Value))
            {
                ServiceResponse finalAmountsValidResponse = AssertAmountGreaterThanZero(stagedCurrency.Amount);
                if (finalAmountsValidResponse.IsSuccess == false)
                {
                    return finalAmountsValidResponse;
                }
            }
            
            // Apply the stagingGround changes to the Repository Currency
            foreach (Currency stagedCurrency in stagingGround.Select(entry => entry.Value))
            {
                Currency globalCurrency = _repository.Get(stagedCurrency.CurrencyType);

                // Calculating if we need to Remove or Add Amount
                int difference = stagedCurrency.Amount - globalCurrency.Amount;
                switch (difference)
                {
                    case > 0:
                        _currencyService.AddAmount(globalCurrency, difference);
                        break;
                    case < 0:
                        _currencyService.RemoveAmount(globalCurrency, -difference);
                        break;
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
        
        /// <summary>
        /// Validates an entire passed array of <see cref="CurrencyTrade"/>s. Will only validate if the <see cref="CurrencyTrade.Amount"/> is above zero
        /// </summary>
        /// <param name="trades">The <see cref="CurrencyTrade"/> array you want to verify</param>
        /// <returns>A <see cref="ServiceResponse"/> object that will tell you if the operation was successful</returns>
        private static ServiceResponse ValidateTrades(params CurrencyTrade[] trades)
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
    }
}
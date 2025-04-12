using IdelPog.Main.Repository;
using IdelPog.Main.Service.Currency;
using IdelPog.Main.Structures;
using IdelPog.Main.Structures.Enums;
using IdelPog.Main.Structures.Models;
using IdelPog.Main.Validation.Assertions.Interfaces;
using IdelPog.Main.Validation.Exceptions;

namespace IdelPog.Main.Orchestration.Currency
{
    /// <summary>
    /// See <see cref="ICurrencyMediator"/> for documentation
    /// </summary>
    public class CurrencyMediator : ICurrencyMediator
    {
        private readonly ICurrencyService _currencyService;
        private readonly IRepository<CurrencyType, Structures.Models.Currency> _repository;
        private readonly IAssertPositive _assert;
        
        public CurrencyMediator(ICurrencyService currencyService, IRepository<CurrencyType, Structures.Models.Currency> repository, IAssertPositive assert)
        {
            _currencyService = currencyService;
            _repository = repository;
            _assert = assert;
        }
        
        public ServiceResponse ProcessCurrencyUpdate(params CurrencyTrade[] trades)
        {
            ServiceResponse validateTradesResponse = ValidateTrades(trades);
            if (validateTradesResponse.IsSuccess == false)
            {
                return validateTradesResponse;
            }
            
            ServiceResponse allCurrenciesExistResponse = AllCurrenciesExist(trades);
            if (allCurrenciesExistResponse.IsSuccess == false)
            {
                return allCurrenciesExistResponse;
            }
            
            Dictionary<CurrencyType, Structures.Models.Currency> stagingGround = new(); 
            Dictionary<CurrencyType, Structures.Models.Currency> originalCurrencies = new(); 

            CloneCurrency(trades, originalCurrencies, stagingGround);
            MutateClonedCurrency(trades, stagingGround);

            ServiceResponse validateFinalAmountsResponse = ValidateFinalAmounts(stagingGround);
            if (validateFinalAmountsResponse.IsSuccess == false)
            {
                return validateFinalAmountsResponse;
            }
            
            ApplyChanges(stagingGround, originalCurrencies);

            return ServiceResponse.Success();
        }
        
        /// <summary>
        /// Will check if all the passed <see cref="CurrencyTrade"/>.<see cref="CurrencyTrade.Currency"/>'s exist
        /// </summary>
        /// <param name="trades">The array you want to check</param>
        /// <returns>A <see cref="ServiceResponse"/> which will tell you if all the <see cref="Currency"/>'s exist</returns>
        private ServiceResponse AllCurrenciesExist(params CurrencyTrade[] trades)
        {
            List<CurrencyType> types = new(); 
            
            foreach (CurrencyTrade currencyTrade in trades)
            {
                if (types.Contains(currencyTrade.Currency))
                {
                    continue;
                }
                
                if (_repository.Contains(currencyTrade.Currency) == false)
                {
                    return ServiceResponse.Failure($"Error! Currency type {currencyTrade.Currency} was not found.");
                }
                
                types.Add(currencyTrade.Currency);
            }
            
            return ServiceResponse.Success();
        }

        /// <summary>
        /// Gets each separate <see cref="Currency"/> from the <see cref="Repository"/>, this is passed into originalCurrencies.
        /// Then, clones these <see cref="Currency"/> retrieved from the <see cref="Repository"/> into the passed stagingGround Dictionary.
        /// </summary>
        /// <param name="currencyTrades">Uses the internal <see cref="CurrencyTrade"/>.<see cref="CurrencyTrade.Currency"/> to Get each <see cref="Currency"/> from the Repository</param>
        /// <param name="originalCurrencies">All the <see cref="Currency"/> returned from Get will first be placed into this Dictionary</param>
        /// <param name="stagingGround">All the <see cref="Currency"/> added into the originalCurrencies Dictionary will be cloned into this</param>
        private void CloneCurrency(CurrencyTrade[] currencyTrades, Dictionary<CurrencyType, Structures.Models.Currency> originalCurrencies, Dictionary<CurrencyType, Structures.Models.Currency> stagingGround)
        {
            foreach (CurrencyTrade currencyTrade in currencyTrades)
            {
                // cloning each Currency, skipping ones we already have gotten
                if (originalCurrencies.ContainsKey(currencyTrade.Currency))
                {
                    // if we already have the Currency, no need to clone it again
                    continue;
                }

                Structures.Models.Currency globalCurrencyClone = _repository.Get(currencyTrade.Currency);
                originalCurrencies.Add(currencyTrade.Currency, globalCurrencyClone);
                    
                // entering each cloned Currency into the stagingGround so we can update them
                stagingGround[currencyTrade.Currency] = new Structures.Models.Currency(globalCurrencyClone.CurrencyType, globalCurrencyClone.Amount);
            }
        }

        /// <summary>
        /// Uses the passed <see cref="CurrencyTrade"/> array properties <see cref="CurrencyTrade.Amount"/> and <see cref="CurrencyTrade.Action"/> to dictate how to update each <see cref="Currency"/>
        /// </summary>
        /// <param name="currencyTrades"><see cref="CurrencyTrade"/></param>
        /// <param name="stagingGround">This Dictionary will now contain each cloned <see cref="Currency"/> from the <see cref="Repository"/></param>
        private void MutateClonedCurrency(CurrencyTrade[] currencyTrades, Dictionary<CurrencyType, Structures.Models.Currency> stagingGround)
        {
            foreach (CurrencyTrade currencyTrade in currencyTrades)
            {
                // Apply CurrencyTrade actions to the stagingGround Currency
                Structures.Models.Currency localCurrency = stagingGround[currencyTrade.Currency];

                switch (currencyTrade.Action)
                {
                    case ActionType.ADD:
                        _currencyService.AddAmount(localCurrency, currencyTrade.Amount);
                        break;
                    case ActionType.REMOVE:
                        _currencyService.RemoveAmount(localCurrency, currencyTrade.Amount);
                        break;
                }
            }
        }
        
        /// <summary>
        /// After each required to update <see cref="Currency"/> has been mutated it is passed into this.
        /// This will check each <see cref="Currency"/> in the passed stagingGround to ensure their <see cref="Currency.Amount"/> is above zero.
        /// Any <see cref="Currency"/> that has a below 0 amount will fail validation thus failing the entire input array
        /// </summary>
        /// <param name="stagingGround">This should now contain each cloned <see cref="Currency"/> from the Repository, but, has had its internal amount updated </param>
        /// <returns>A <see cref="ServiceResponse"/> who's <see cref="ServiceResponse.IsSuccess"/> will tell if you all the passed trades pass validation</returns>
        private ServiceResponse ValidateFinalAmounts(Dictionary<CurrencyType, Structures.Models.Currency> stagingGround)
        {
            ServiceResponse serviceResponse = AssertArrayIsPositive(stagingGround.Select(entry => entry.Value.Amount).ToArray());
            
            return serviceResponse;
        }

        /// <summary>
        /// After we pass validation we now Update the <see cref="Currency"/> in the Repository to the new state.
        /// By this point, all data should be validated so nothing is checked.
        /// </summary>
        /// <param name="stagingGround">These <see cref="Currency"/> should now be different from the ones retrieved from the Repository</param>
        /// <param name="originalCurrencies">This contains the original retrieved <see cref="Currency"/> that has not been changed</param>
        private void ApplyChanges(Dictionary<CurrencyType, Structures.Models.Currency> stagingGround, Dictionary<CurrencyType, Structures.Models.Currency> originalCurrencies)
        {
            foreach (Structures.Models.Currency stagedCurrency in stagingGround.Select(entry => entry.Value))
            {
                Structures.Models.Currency globalCurrency = originalCurrencies[stagedCurrency.CurrencyType];

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

                _repository.Update(globalCurrency.CurrencyType, globalCurrency);
            }
        }
        
        /// <summary>
        /// Validates an entire passed array of <see cref="CurrencyTrade"/>s. Will only validate if the <see cref="CurrencyTrade.Amount"/> is above zero
        /// </summary>
        /// <param name="trades">The <see cref="CurrencyTrade"/> array you want to verify</param>
        /// <returns>A <see cref="ServiceResponse"/> object that will tell you if the operation was successful</returns>
        private ServiceResponse ValidateTrades(params CurrencyTrade[] trades)
        {
            ServiceResponse serviceResponse = AssertArrayIsPositive(trades.Select(entry => entry.Amount).ToArray());
            
            return serviceResponse;
        }
        
        /// <summary>
        /// Asserts that every number is positive
        /// </summary>
        /// <param name="numbers">The numbers you want to validate</param>
        /// <returns>A <see cref="ServiceResponse"/> object on the state of the assertion</returns>
        private ServiceResponse AssertArrayIsPositive(int[] numbers)
        {
            try
            {
                _assert.AssertNumberIsPositive(numbers);
                return ServiceResponse.Success();
            }
            catch (NegativeNumberException exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }
        }
    }
}
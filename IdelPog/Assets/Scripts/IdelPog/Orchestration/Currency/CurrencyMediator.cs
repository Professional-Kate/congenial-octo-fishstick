using System.Collections.Generic;
using System.Linq;
using IdelPog.Exceptions;
using IdelPog.Model;
using IdelPog.Repository;
using IdelPog.Service;
using IdelPog.Structures;
using IdelPog.Structures.Enums;

namespace IdelPog.Orchestration
{
    /// <summary>
    /// See <see cref="ICurrencyMediator"/> for documentation
    /// </summary>
    /// <seealso cref="CreateDefault"/>
    public class CurrencyMediator : ICurrencyMediator
    {
        private readonly ICurrencyService _currencyService;
        private readonly IRepository<CurrencyType, Currency> _repository;

        public CurrencyMediator(ICurrencyService currencyService, IRepository<CurrencyType, Currency> repository)
        {
            _currencyService = currencyService;
            _repository = repository;
        }

        /// <summary>
        /// Creates a <see cref="ICurrencyMediator"/> with all required dependencies
        /// </summary>
        /// <returns>A new <see cref="ICurrencyMediator"/> class with all dependencies resolved</returns>
        public static ICurrencyMediator CreateDefault()
        {
            ICurrencyService service = new CurrencyService();
            IRepository<CurrencyType, Currency> repository = new Repository<CurrencyType, Currency>();

            return new CurrencyMediator(service, repository);
        }
        
        public ServiceResponse ProcessCurrencyUpdate(params CurrencyTrade[] trades)
        {
            ServiceResponse validateTradesResponse = ValidateTrades(trades);
            if (validateTradesResponse.IsSuccess == false)
            {
                return validateTradesResponse;
            }
            
            Dictionary<CurrencyType, Currency> stagingGround = new(); 
            Dictionary<CurrencyType, Currency> originalCurrencies = new(); 

            try
            {
                CloneCurrency(trades, originalCurrencies, stagingGround);
                MutateClonedCurrency(trades, stagingGround);
            }
            catch (NotFoundException exception)
            {
                return ServiceResponse.Failure(exception.Message);
            }

            ServiceResponse validateFinalAmountsResponse = ValidateFinalAmounts(stagingGround);
            if (validateFinalAmountsResponse.IsSuccess == false)
            {
                return validateFinalAmountsResponse;
            }
            
            ApplyChanges(stagingGround, originalCurrencies);

            return ServiceResponse.Success();
        }

        /// <summary>
        /// Gets each separate <see cref="Currency"/> from the <see cref="CurrencyRepository"/>, this is passed into originalCurrencies.
        /// Then, clones these <see cref="Currency"/> retrieved from the <see cref="CurrencyRepository"/> into the passed stagingGround Dictionary.
        /// </summary>
        /// <param name="currencyTrades">Uses the internal <see cref="CurrencyTrade"/>.<see cref="CurrencyTrade.Currency"/> to Get each <see cref="Currency"/> from the Repository</param>
        /// <param name="originalCurrencies">All the <see cref="Currency"/> returned from Get will first be placed into this Dictionary</param>
        /// <param name="stagingGround">All the <see cref="Currency"/> added into the originalCurrencies Dictionary will be cloned into this</param>
        private void CloneCurrency(CurrencyTrade[] currencyTrades, Dictionary<CurrencyType, Currency> originalCurrencies, Dictionary<CurrencyType, Currency> stagingGround)
        {
            foreach (CurrencyTrade currencyTrade in currencyTrades)
            {
                if (!originalCurrencies.TryGetValue(currencyTrade.Currency, out Currency originalCurrency))
                {
                    originalCurrency = _repository.Get(currencyTrade.Currency);
                    originalCurrencies.Add(currencyTrade.Currency, originalCurrency);
                }

                // Clone and stage
                if (!stagingGround.ContainsKey(currencyTrade.Currency))
                {
                    stagingGround[currencyTrade.Currency] = new Currency(originalCurrency.CurrencyType, originalCurrency.Amount);
                }
            }
        }

        /// <summary>
        /// Uses the passed <see cref="CurrencyTrade"/> array properties <see cref="CurrencyTrade.Amount"/> and <see cref="CurrencyTrade.Action"/> to dictate how to update each <see cref="Currency"/>
        /// </summary>
        /// <param name="currencyTrades"><see cref="CurrencyTrade"/></param>
        /// <param name="stagingGround">This Dictionary will now contain each cloned <see cref="Currency"/> from the <see cref="CurrencyRepository"/></param>
        private void MutateClonedCurrency(CurrencyTrade[] currencyTrades, Dictionary<CurrencyType, Currency> stagingGround)
        {
            foreach (CurrencyTrade currencyTrade in currencyTrades)
            {
                // Apply CurrencyTrade actions to the stagingGround Currency
                Currency localCurrency = stagingGround[currencyTrade.Currency];

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
        private static ServiceResponse ValidateFinalAmounts(Dictionary<CurrencyType, Currency> stagingGround)
        {
            ServiceResponse finalAmountsValidResponse = AssertAmountGreaterThanZero(stagingGround.Select(entry => entry.Value.Amount));
            if (finalAmountsValidResponse.IsSuccess == false)
            {
                return finalAmountsValidResponse;
            }
        
            return ServiceResponse.Success();
        }

        /// <summary>
        /// After we pass validation we now Update the <see cref="Currency"/> in the Repository to the new state.
        /// By this point, all data should be validated so nothing is checked.
        /// </summary>
        /// <param name="stagingGround">These <see cref="Currency"/> should now be different from the ones retrieved from the Repository</param>
        /// <param name="originalCurrencies">This contains the original retrieved <see cref="Currency"/> that has not been changed</param>
        private void ApplyChanges(Dictionary<CurrencyType, Currency> stagingGround, Dictionary<CurrencyType, Currency> originalCurrencies)
        {
            foreach (Currency stagedCurrency in stagingGround.Select(entry => entry.Value))
            {
                Currency globalCurrency = originalCurrencies[stagedCurrency.CurrencyType];

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
                    case 0:
                        // no change needed, obviously.
                        break;
                }

                _repository.Update(globalCurrency.CurrencyType, globalCurrency);
            }
        }
        
        /// <summary>
        /// Asserts that every passed int is greater than 0.
        /// </summary>
        /// <param name="amounts">The amounts you want to check</param>
        /// <returns>
        /// <returns>A <see cref="ServiceResponse"/> who's <see cref="ServiceResponse.IsSuccess"/> will tell if you all the passed numbers passed validation</returns>
        /// </returns>
        private static ServiceResponse AssertAmountGreaterThanZero(IEnumerable<int> amounts)
        {
            foreach (int amount in amounts)
            {
                if (amount <= 0)
                {
                    return ServiceResponse.Failure($"Error! Passed amount : '{amount}' is required to be a positive number.");
                }
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
            ServiceResponse serviceResponse = AssertAmountGreaterThanZero(trades.Select(entry => entry.Amount));
            if (serviceResponse.IsSuccess == false)
            {
                return serviceResponse;
            }

            return ServiceResponse.Success();
        }
    }
}
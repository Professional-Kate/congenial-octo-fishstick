using IdelPog.Common.Repository;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Dispatchers;
using IdelPog.SimulationEngine.Structures;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.SimulationEngine.Currency
{
    /// <inheritdoc cref="ICurrencyUpdateMediator"/>
    public class CurrencyUpdateMediator : ICurrencyUpdateMediator
    {
        private readonly ICurrencyService _currencyService;
        private readonly IStateRepository<CurrencyType, Currency> _currencyRepository;
        private readonly ICurrencyUpdateDispatcher _currencyUpdateDispatcher;
        private readonly IAssertPositive _assertPositive;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;
        private readonly IAssertFound _assertFound;
        
        public CurrencyUpdateMediator(ICurrencyService currencyService, IStateRepository<CurrencyType, Currency> stateRepository, ICurrencyUpdateDispatcher currencyUpdateDispatcher, IAssertPositive assertPositive, IAssertCollectionNotEmpty assertCollectionNotEmpty, IAssertFound assertFound)
        {
            _currencyService = currencyService;
            _currencyRepository = stateRepository;
            _currencyUpdateDispatcher = currencyUpdateDispatcher;
            _assertPositive = assertPositive;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
            _assertFound = assertFound;
        }
        
        public void ProcessCurrencyUpdate(IReadOnlyList<CurrencyUpdate> trades)
        {
            // validate trades
            _assertCollectionNotEmpty.Handle(trades);
            _assertPositive.AssertNumberIsPositive(trades.Select(entry => entry.Amount).ToArray());
            
            AllCurrenciesExist(trades);
            
            Dictionary<CurrencyType, Currency> stagingGround = new(); 
            Dictionary<CurrencyType, Currency> originalCurrencies = new(); 

            CloneCurrency(trades, originalCurrencies, stagingGround);
            MutateClonedCurrency(trades, stagingGround);

            // validate final amounts
            _assertPositive.AssertNumberIsPositive(stagingGround.Select(entry => entry.Value.Amount).ToArray());
            
            ApplyChanges(stagingGround, originalCurrencies);
            
            _currencyUpdateDispatcher.Dispatch(trades);
        }

        private void AllCurrenciesExist(IReadOnlyList<CurrencyUpdate> trades)
        {
            List<CurrencyType> types = []; 
            
            foreach (CurrencyUpdate currencyTrade in trades)
            {
                if (types.Contains(currencyTrade.CurrencyType))
                {
                    continue;
                }
                
                _assertFound.AssertItemIsFound(currencyTrade.CurrencyType,() => _currencyRepository.Contains(currencyTrade.CurrencyType));
                types.Add(currencyTrade.CurrencyType);
            }
        }

        /// <summary>
        /// Gets each separate <see cref="Currency"/> from the <see cref="StateRepository{TID,T}"/>, this is passed into originalCurrencies.
        /// Then, clones these <see cref="Currency"/> retrieved from the <see cref="StateRepository{TID,T}"/> into the passed stagingGround Dictionary.
        /// </summary>
        /// <param name="currencyTrades">Uses the internal <see cref="CurrencyUpdate"/>.<see cref="CurrencyUpdate.CurrencyType"/> to Get each <see cref="Currency"/> from the Repository</param>
        /// <param name="originalCurrencies">All the <see cref="Currency"/> returned from Get will first be placed into this Dictionary</param>
        /// <param name="stagingGround">All the <see cref="Currency"/> added into the originalCurrencies Dictionary will be cloned into this</param>
        private void CloneCurrency(IReadOnlyList<CurrencyUpdate> currencyTrades, Dictionary<CurrencyType, Currency> originalCurrencies, Dictionary<CurrencyType, Currency> stagingGround)
        {
            foreach (CurrencyUpdate currencyTrade in currencyTrades)
            {
                // cloning each Currency, skipping ones we already have gotten
                if (originalCurrencies.ContainsKey(currencyTrade.CurrencyType))
                {
                    // if we already have the Currency, no need to clone it again
                    continue;
                }

                Currency globalCurrencyClone = _currencyRepository.Get(currencyTrade.CurrencyType);
                originalCurrencies.Add(currencyTrade.CurrencyType, globalCurrencyClone);
                    
                // entering each cloned Currency into the stagingGround so we can update them
                stagingGround[currencyTrade.CurrencyType] = new Currency(globalCurrencyClone.CurrencyType, globalCurrencyClone.Amount);
            }
        }

        /// <summary>
        /// Uses the passed <see cref="CurrencyUpdate"/> array properties <see cref="CurrencyUpdate.Amount"/> and <see cref="CurrencyUpdate.Action"/> to dictate how to update each <see cref="Currency"/>
        /// </summary>
        /// <param name="currencyTrades"><see cref="CurrencyUpdate"/></param>
        /// <param name="stagingGround">This Dictionary will now contain each cloned <see cref="Currency"/> from the <see cref="StateRepository{TID,T}"/></param>
        private void MutateClonedCurrency(IReadOnlyList<CurrencyUpdate> currencyTrades, Dictionary<CurrencyType, Currency> stagingGround)
        {
            foreach (CurrencyUpdate currencyTrade in currencyTrades)
            {
                // Apply CurrencyTrade actions to the stagingGround Currency
                Currency localCurrency = stagingGround[currencyTrade.CurrencyType];

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
                }

                _currencyRepository.Update(globalCurrency.CurrencyType, globalCurrency);
            }
        }
    }
}
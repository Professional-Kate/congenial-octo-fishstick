using IdelPog.Common.Repository;
using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Dispatchers;
using IdelPog.SimulationEngine.Structures;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.SimulationEngine.Currency
{
    /// <inheritdoc cref="ICurrencyUpdateMediator"/>
    public class CurrencyUpdateMediator(ICurrencyService currencyService, IStateRepository<CurrencyType, Currency> stateRepository, ICurrencyUpdateDispatcher currencyUpdateDispatcher, IAssertPositive assert, IAssertCollectionNotEmpty assertCollectionNotEmpty)
        : ICurrencyUpdateMediator
    {
        public void ProcessCurrencyUpdate(IReadOnlyList<CurrencyUpdate> trades)
        {
            // validate trades
            assertCollectionNotEmpty.Handle(trades);
            assert.AssertNumberIsPositive(trades.Select(entry => entry.Amount).ToArray());
            
            AllCurrenciesExist(trades);
            
            Dictionary<CurrencyType, Currency> stagingGround = new(); 
            Dictionary<CurrencyType, Currency> originalCurrencies = new(); 

            CloneCurrency(trades, originalCurrencies, stagingGround);
            MutateClonedCurrency(trades, stagingGround);

            // validate final amounts
            assert.AssertNumberIsPositive(stagingGround.Select(entry => entry.Value.Amount).ToArray());
            
            ApplyChanges(stagingGround, originalCurrencies);
            
            currencyUpdateDispatcher.Dispatch(trades);
        }

        private void AllCurrenciesExist(IReadOnlyList<CurrencyUpdate> trades)
        {
            List<CurrencyType> types = []; 
            
            foreach (CurrencyUpdate currencyTrade in trades)
            {
                if (types.Contains(currencyTrade.Currency))
                {
                    continue;
                }
                
                if (stateRepository.Contains(currencyTrade.Currency) == false)
                {
                    throw new Exception();
                }
                
                types.Add(currencyTrade.Currency);
            }
        }

        /// <summary>
        /// Gets each separate <see cref="Currency"/> from the <see cref="StateRepository{TID,T}"/>, this is passed into originalCurrencies.
        /// Then, clones these <see cref="Currency"/> retrieved from the <see cref="StateRepository{TID,T}"/> into the passed stagingGround Dictionary.
        /// </summary>
        /// <param name="currencyTrades">Uses the internal <see cref="CurrencyUpdate"/>.<see cref="CurrencyUpdate.Currency"/> to Get each <see cref="Currency"/> from the Repository</param>
        /// <param name="originalCurrencies">All the <see cref="Currency"/> returned from Get will first be placed into this Dictionary</param>
        /// <param name="stagingGround">All the <see cref="Currency"/> added into the originalCurrencies Dictionary will be cloned into this</param>
        private void CloneCurrency(IReadOnlyList<CurrencyUpdate> currencyTrades, Dictionary<CurrencyType, Currency> originalCurrencies, Dictionary<CurrencyType, Currency> stagingGround)
        {
            foreach (CurrencyUpdate currencyTrade in currencyTrades)
            {
                // cloning each Currency, skipping ones we already have gotten
                if (originalCurrencies.ContainsKey(currencyTrade.Currency))
                {
                    // if we already have the Currency, no need to clone it again
                    continue;
                }

                Currency globalCurrencyClone = stateRepository.Get(currencyTrade.Currency);
                originalCurrencies.Add(currencyTrade.Currency, globalCurrencyClone);
                    
                // entering each cloned Currency into the stagingGround so we can update them
                stagingGround[currencyTrade.Currency] = new Currency(globalCurrencyClone.CurrencyType, globalCurrencyClone.Amount);
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
                Currency localCurrency = stagingGround[currencyTrade.Currency];

                switch (currencyTrade.Action)
                {
                    case ActionType.ADD:
                        currencyService.AddAmount(localCurrency, currencyTrade.Amount);
                        break;
                    case ActionType.REMOVE:
                        currencyService.RemoveAmount(localCurrency, currencyTrade.Amount);
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
                        currencyService.AddAmount(globalCurrency, difference);
                        break;
                    case < 0:
                        currencyService.RemoveAmount(globalCurrency, -difference);
                        break;
                }

                stateRepository.Update(globalCurrency.CurrencyType, globalCurrency);
            }
        }
    }
}
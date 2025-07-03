using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Dispatchers;
using IdelPog.SimulationEngine.Currency.Factories;
using IdelPog.SimulationEngine.Structures;
using IdelPog.Validation.Assertions.Interfaces;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyUpdateSummarizer : ICurrencyUpdateSummarizer
    {
        private readonly ICurrencyUpdateFactory _currencyUpdateFactory;
        private readonly IAssertPositive _assertPositive;

        public CurrencyUpdateSummarizer(ICurrencyUpdateFactory currencyUpdateFactory, IAssertPositive assertPositive)
        {
            _currencyUpdateFactory = currencyUpdateFactory;
            _assertPositive = assertPositive;
        }
        
        public CurrencyUpdate[] GetSummary(IReadOnlyList<CurrencyUpdate> updates)
        {
            Dictionary<CurrencyType, int> amounts = [];

            foreach (CurrencyUpdate currencyUpdate in updates)
            {
                _assertPositive.AssertNumberIsPositive(currencyUpdate.Amount);

                if (amounts.ContainsKey(currencyUpdate.Currency))
                {
                    switch (currencyUpdate.Action)
                    {
                        case ActionType.ADD:
                            amounts[currencyUpdate.Currency] += currencyUpdate.Amount;
                            break;
                        case ActionType.REMOVE:
                            amounts[currencyUpdate.Currency] -= currencyUpdate.Amount;
                            break;
                    }

                    continue;
                }
                
                amounts.Add(currencyUpdate.Currency, currencyUpdate.Amount);
            }

            List<CurrencyUpdate> summaryUpdates = [];
            foreach ((CurrencyType currencyType, int amount) in amounts)
            {
                if (amount == 0)
                {
                    continue;
                }
                
                ActionType action;

                if (amount < 0)
                {
                    action = ActionType.REMOVE;
                }
                else
                {
                    action = ActionType.ADD;
                }
                
                summaryUpdates.Add(_currencyUpdateFactory.CreateCurrencyUpdate(currencyType, action, amount));
            }
            
            return summaryUpdates.ToArray();
        }
    }
}
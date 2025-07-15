using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Factories;
using IdelPog.SimulationEngine.Structures;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyUpdateSummarizer : ICurrencyUpdateSummarizer
    {
        private readonly ICurrencyUpdateFactory _currencyUpdateFactory;
        private readonly IAssertPositive _assertPositive;
        private readonly IAssertNotNull _assertNotNull;
        private readonly IAssertCollectionNotEmpty _assertCollectionNotEmpty;

        public CurrencyUpdateSummarizer(ICurrencyUpdateFactory currencyUpdateFactory, IAssertPositive assertPositive,  IAssertNotNull assertNotNull, IAssertCollectionNotEmpty assertCollectionNotEmpty)
        {
            _currencyUpdateFactory = currencyUpdateFactory;
            _assertPositive = assertPositive;
            _assertNotNull = assertNotNull;
            _assertCollectionNotEmpty = assertCollectionNotEmpty;
        }
        
        public CurrencyUpdate[] GetSummary(IReadOnlyList<CurrencyUpdate> updates)
        {
            _assertNotNull.AssertObjectNotNull(updates);
            _assertCollectionNotEmpty.Handle(updates);
            
            Dictionary<CurrencyType, int> amounts =  SummarizeAmounts(updates);
            List<CurrencyUpdate> summaryUpdates = CreateSummaryUpdates(amounts);
            
            return summaryUpdates.ToArray();
        }

        private Dictionary<CurrencyType, int> SummarizeAmounts(IReadOnlyList<CurrencyUpdate> updates)
        {
            Dictionary<CurrencyType, int> amounts = new();
            
            foreach (CurrencyUpdate currencyUpdate in updates)
            {
                _assertPositive.AssertNumberIsPositive<CurrencyUpdate>(currencyUpdate.Amount);

                if (amounts.ContainsKey(currencyUpdate.CurrencyType) == false)
                {
                    amounts.Add(currencyUpdate.CurrencyType, 0);
                }
                
                switch (currencyUpdate.Action)
                {
                    case ActionType.ADD:
                        amounts[currencyUpdate.CurrencyType] += currencyUpdate.Amount;
                        break;
                    case ActionType.REMOVE:
                        amounts[currencyUpdate.CurrencyType] -= currencyUpdate.Amount;
                        break;
                }
            }
            
            return amounts;
        }
        
        private List<CurrencyUpdate> CreateSummaryUpdates(Dictionary<CurrencyType, int> amounts)
        {
            List<CurrencyUpdate> updates = [];
            
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
                
                updates.Add(_currencyUpdateFactory.CreateCurrencyUpdate(currencyType, action, Math.Abs(amount)));
            }

            return updates;
        }
    }
}
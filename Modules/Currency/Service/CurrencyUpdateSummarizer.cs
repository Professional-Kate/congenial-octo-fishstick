using IdelPog.Core.Contracts;
using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation.Assertion.Interface;
using IdelPog.Currency.Factory.Interface;
using IdelPog.Currency.Service.Interface;

namespace IdelPog.Currency.Service
{
    public class CurrencyUpdateSummarizer : ICurrencyUpdateSummarizer
    {
        private readonly ICurrencyUpdateFactory _currencyUpdateFactory;
        private readonly ICollectionAssertion _collectionAssertion;

        public CurrencyUpdateSummarizer(ICurrencyUpdateFactory currencyUpdateFactory, ICollectionAssertion collectionAssertion)
        {
            _currencyUpdateFactory = currencyUpdateFactory;
            _collectionAssertion = collectionAssertion;
        }

        public CurrencyUpdate[] GetSummary(IReadOnlyList<CurrencyUpdate> updates)
        {
            _collectionAssertion.AssertHasElements(updates);

            Dictionary<CurrencyType, RunningUpdate> amounts = SummarizeAmounts(updates);
            List<CurrencyUpdate> summaryUpdates = CreateSummaryUpdates(amounts);

            return summaryUpdates.ToArray();
        }

        private Dictionary<CurrencyType, RunningUpdate> SummarizeAmounts(IReadOnlyList<CurrencyUpdate> updates)
        {
            Dictionary<CurrencyType, RunningUpdate> amounts = new();

            foreach (CurrencyUpdate currencyUpdate in updates)
            {
                amounts.TryAdd(currencyUpdate.CurrencyType, new RunningUpdate());
                amounts[currencyUpdate.CurrencyType].Apply(currencyUpdate.ActionType, currencyUpdate.Amount);
            }

            return amounts;
        }

        private List<CurrencyUpdate> CreateSummaryUpdates(Dictionary<CurrencyType, RunningUpdate> amounts)
        {
            List<CurrencyUpdate> updates = [];

            foreach ((CurrencyType currencyType, RunningUpdate runningAmount) in amounts)
            {
                if (runningAmount.IsZeroAmount())
                {
                    continue;
                }

                ActionType action;
                uint currencyUpdateAmount = runningAmount.AddAmount;

                if (runningAmount.RemoveAmount > currencyUpdateAmount)
                {
                    action = ActionType.REMOVE;
                    currencyUpdateAmount = runningAmount.RemoveAmount - runningAmount.AddAmount;
                }
                else
                {
                    action = ActionType.ADD;
                    currencyUpdateAmount -=  runningAmount.RemoveAmount;
                }
                
                updates.Add(_currencyUpdateFactory.CreateCurrencyUpdate(action, currencyUpdateAmount, currencyType));
            }

            return updates;
        }
    }
}
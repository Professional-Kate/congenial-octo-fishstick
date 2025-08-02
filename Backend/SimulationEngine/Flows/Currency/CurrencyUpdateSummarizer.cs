using IdelPog.Common.Commands;
using IdelPog.Common.Enums;
using IdelPog.Common.Factories;
using IdelPog.Validation.Assertions;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyUpdateSummarizer : ICurrencyUpdateSummarizer
    {
        private readonly ICurrencyUpdateFactory _currencyUpdateFactory;
        private readonly IObjectNullAssertion _objectNullAssertion;
        private readonly ICollectionAssertion _collectionAssertion;

        public CurrencyUpdateSummarizer(ICurrencyUpdateFactory currencyUpdateFactory, IObjectNullAssertion objectNullAssertion,
            ICollectionAssertion collectionAssertion)
        {
            _currencyUpdateFactory = currencyUpdateFactory;
            _objectNullAssertion = objectNullAssertion;
            _collectionAssertion = collectionAssertion;
        }

        public CurrencyUpdate[] GetSummary(IReadOnlyList<CurrencyUpdate> updates)
        {
            _objectNullAssertion.AssertNotNull(updates, nameof(updates));
            _collectionAssertion.AssertNotEmpty(updates);

            Dictionary<CurrencyType, CurrencyRunningUpdate> amounts = SummarizeAmounts(updates);
            List<CurrencyUpdate> summaryUpdates = CreateSummaryUpdates(amounts);

            return summaryUpdates.ToArray();
        }

        private Dictionary<CurrencyType, CurrencyRunningUpdate> SummarizeAmounts(IReadOnlyList<CurrencyUpdate> updates)
        {
            Dictionary<CurrencyType, CurrencyRunningUpdate> amounts = new();

            foreach (CurrencyUpdate currencyUpdate in updates)
            {
                amounts.TryAdd(currencyUpdate.CurrencyType, new CurrencyRunningUpdate());
                amounts[currencyUpdate.CurrencyType].Apply(currencyUpdate.Action, currencyUpdate.Amount);
            }

            return amounts;
        }

        private List<CurrencyUpdate> CreateSummaryUpdates(Dictionary<CurrencyType, CurrencyRunningUpdate> amounts)
        {
            List<CurrencyUpdate> updates = [];

            foreach ((CurrencyType currencyType, CurrencyRunningUpdate runningAmount) in amounts)
            {
                if (runningAmount.AddAmount == runningAmount.RemoveAmount)
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

    /// <summary>
    /// Internal helper class for tracking the running total of add/remove calls
    /// </summary>
    internal sealed class CurrencyRunningUpdate
    {
        internal uint AddAmount { get; private set; }
        internal uint RemoveAmount { get; private set; }
        
        /// <summary>
        /// Add an amount to the internal properties
        /// </summary>
        /// <param name="action">If this is a remove / add amount update</param>
        /// <param name="amount">The amount you want to add</param>
        /// <exception cref="ArgumentOutOfRangeException">If the action isn't ADD/REMOVE</exception>
        internal void Apply(ActionType action, uint amount)
        {
            switch (action)
            {
                case ActionType.ADD:
                    AddAmount += amount;
                    break;
                case ActionType.REMOVE:
                    RemoveAmount += amount;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(action), action, null);
            }
        }
    }
}
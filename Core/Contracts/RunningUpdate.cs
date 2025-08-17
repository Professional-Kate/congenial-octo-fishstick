using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Core.Contracts
{
    public sealed class RunningUpdate
    {
        public uint AddAmount { get; private set; }
        public uint RemoveAmount { get; private set; }
        
        /// <summary>
        /// Add an amount to the internal properties
        /// </summary>
        /// <param name="action">If this is a remove / add amount update</param>
        /// <param name="amount">The amount you want to add</param>
        /// <exception cref="ArgumentOutOfRangeException">If the action isn't ADD/REMOVE</exception>
        public void Apply(ActionType action, uint amount)
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

        public bool IsZeroAmount()
        {
            return AddAmount == RemoveAmount;
        }
    }
}
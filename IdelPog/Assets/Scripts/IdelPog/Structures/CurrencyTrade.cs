using IdelPog.Structures.Enums;

namespace IdelPog.Structures
{
    /// <summary>
    /// This structure is used to update a Currency model. 
    /// </summary>
    /// <remarks>
    /// Uses the internal <see cref="CurrencyType"/> to dictate what Currency to update, this update is decided by the <see cref="ActionType"/>
    /// </remarks>
    /// <seealso cref="Amount"/>
    /// <seealso cref="Currency"/>
    /// <seealso cref="Action"/>
    public struct CurrencyTrade
    {
        /// <summary>
        /// The amount that should be Removed/Added to the <see cref="CurrencyType"/>
        /// </summary>
        public int Amount { get; }

        /// <summary>
        /// What <see cref="CurrencyType"/> the action should perform on
        /// </summary>
        public CurrencyType Currency { get; }
        
        /// <summary>
        /// <inheritdoc cref="ActionType"/>
        /// </summary>
        public ActionType Action { get; }

        public CurrencyTrade(int amount, CurrencyType currency, ActionType action)
        {
            Amount = amount;
            Currency = currency;
            Action = action;
        }
    }
}
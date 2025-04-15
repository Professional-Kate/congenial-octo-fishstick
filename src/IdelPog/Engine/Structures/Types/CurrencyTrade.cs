using IdelPog.Engine.Structures.Enums;

namespace IdelPog.Engine.Structures.Types
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
    public struct CurrencyTrade(int amount, CurrencyType currency, ActionType action)
    {
        /// <summary>
        /// The amount that should be Removed/Added to the <see cref="CurrencyType"/>
        /// </summary>
        public int Amount { get; } = amount;

        /// <summary>
        /// What <see cref="CurrencyType"/> the action should perform on
        /// </summary>
        public CurrencyType Currency { get; } = currency;

        /// <summary>
        /// <inheritdoc cref="ActionType"/>
        /// </summary>
        public ActionType Action { get; } = action;
    }
}
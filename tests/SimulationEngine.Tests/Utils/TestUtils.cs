using IdelPog.SimulationEngine.Currency;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Structures;

namespace IdelPogTests.Utils
{
    /// <summary>
    /// Commonly used helper test methods. 
    /// </summary>
    /// <seealso cref="CreateTrade"/>
    internal static class TestUtils
    {
        /// <summary>
        /// Creates a <see cref="CurrencyUpdate"/> object and returns it
        /// </summary>
        /// <param name="amount">The amount to modify</param>
        /// <param name="type">The <see cref="CurrencyType"/> you want to modify</param>
        /// <param name="action">The <see cref="ActionType"/></param>
        /// <returns>The created <see cref="CurrencyUpdate"/></returns>
        internal static CurrencyUpdate CreateTrade(int amount, CurrencyType type, ActionType action)
        {
            return new CurrencyUpdate
            {
                Amount = amount,
                Currency = type,
                Action = action
            };
        }
    }
}
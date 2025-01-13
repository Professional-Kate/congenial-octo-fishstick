using IdelPog.Structures;
using IdelPog.Structures.Enums;

namespace Tests.Utils
{
    /// <summary>
    /// Commonly used helper test methods. 
    /// </summary>
    /// <seealso cref="CreateTrade"/>
    internal static class TestUtils
    {
        /// <summary>
        /// Creates a <see cref="CurrencyTrade"/> object and returns it
        /// </summary>
        /// <param name="amount">The amount to modify</param>
        /// <param name="type">The <see cref="CurrencyType"/> you want to modify</param>
        /// <param name="action">The <see cref="ActionType"/></param>
        /// <returns>The created <see cref="CurrencyTrade"/></returns>
        internal static CurrencyTrade CreateTrade(int amount, CurrencyType type, ActionType action)
        {
            return new CurrencyTrade(amount, type, action);
        }
    }
}
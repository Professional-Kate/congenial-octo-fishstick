using IdelPog.Structures;
using IdelPog.Structures.Enums;

namespace IdelPog.Controller
{
    /// <seealso cref="UpdateCurrency"/>
    public interface ICurrencyController
    {
        /// <summary>
        /// Uses a passed <see cref="CurrencyTrade"/> to dictate how to update a Currency model with matching <see cref="CurrencyType"/>
        /// </summary>
        /// <param name="trades">Can be a singular <see cref="CurrencyTrade"/> or an array  of <see cref="CurrencyTrade"/>s</param>
        /// <remarks>
        /// Every implementation of this method is required to take a single, or an array of <see cref="CurrencyTrade"/>s
        /// </remarks>
        public void UpdateCurrency(params CurrencyTrade[] trades);
    }
}
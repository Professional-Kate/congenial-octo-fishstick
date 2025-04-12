using IdelPog.Engine.Structures.Enums;
using IdelPog.Engine.Structures.Types;

namespace IdelPog.Engine.Controller.Currency
{
    /// <seealso cref="UpdateCurrency"/>
    public interface ICurrencyController
    {
        /// <summary>
        /// Uses a passed <see cref="CurrencyTrade"/> to dictate how to update a Currency model with matching <see cref="CurrencyType"/>
        /// </summary>
        /// <param name="trades">Can be a singular <see cref="CurrencyTrade"/> or an array  of <see cref="CurrencyTrade"/>s</param>
        /// <returns>A <see cref="ServiceResponse"/> object on the state of the operation</returns>
        /// <remarks>
        /// Every implementation of this method is required to take a single, or an array of <see cref="CurrencyTrade"/>s
        /// </remarks>
        public ServiceResponse UpdateCurrency(params CurrencyTrade[] trades);
    }
}
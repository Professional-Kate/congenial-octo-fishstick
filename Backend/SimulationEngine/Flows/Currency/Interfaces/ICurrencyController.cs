using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Currency
{
    /// <seealso cref="UpdateCurrency"/>
    public interface ICurrencyController
    {
        /// <summary>
        /// Uses a passed <see cref="CurrencyTrade"/> to dictate how to update a Currency model with matching <see cref="CurrencyType"/>
        /// </summary>
        /// <param name="trades">A <see cref="IReadOnlyList{T}"/> containing all the trades you want processed</param>
        /// <returns>A <see cref="ServiceResponse"/> object on the state of the operation</returns>
        /// <remarks>
        /// Every implementation of this method is required to take a single, or an array of <see cref="CurrencyTrade"/>s
        /// </remarks>
        public ServiceResponse UpdateCurrency(IReadOnlyList<CurrencyTrade> trades);
    }
}
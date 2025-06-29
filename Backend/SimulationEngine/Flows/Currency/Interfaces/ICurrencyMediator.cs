using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Structures.Types;

namespace IdelPog.SimulationEngine.Currency
{
    /// <seealso cref="ProcessCurrencyUpdate"/>
    public interface ICurrencyMediator
    {
        /// <summary>
        /// Uses a passed <see cref="CurrencyTrade"/> array, or a singular <see cref="CurrencyTrade"/> to modify a <see cref="Currency"/> model
        /// </summary>
        /// <param name="trades">An array of <see cref="CurrencyTrade"/>s will dictate what <see cref="Currency"/> to update with how much amount and what action</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>The total <see cref="CurrencyTrade"/> array will need to leave <see cref="Currency"/> in a correct state. <see cref="Currency.Amount"/> cannot be less than or equal to 0</item>
        /// <item>If one <see cref="CurrencyTrade"/> will leave any <see cref="Currency"/> in a non-correct state, the whole array won't be processed</item>
        /// <item>The specific order of the <see cref="CurrencyTrade"/>[] doesn't matter</item>
        /// <item>A <see cref="ServiceResponse"/>.<see cref="ServiceResponse.Failure"/> will be returned if any <see cref="CurrencyTrade"/>.<see cref="CurrencyTrade.Amount"/> in the passed array is equal to or less than 0</item>
        /// </list>
        /// </remarks>
        public void ProcessCurrencyUpdate(IReadOnlyList<CurrencyTrade> trades);
        
        public void CreateCurrency(IReadOnlyList<CurrencyCreation> currencies);
    }
}
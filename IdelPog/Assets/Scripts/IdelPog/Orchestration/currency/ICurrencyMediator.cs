using IdelPog.Service;
using IdelPog.Structures;

namespace IdelPog.Orchestration
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ProcessCurrencyUpdate"/>
    public interface ICurrencyMediator
    {
        /// <summary>
        /// Uses a passed <see cref="CurrencyTrade"/> array to modify a <see cref="Model.Currency"/> model
        /// </summary>
        /// <param name="trades">An array of <see cref="CurrencyTrade"/>s will dictate what <see cref="Model.Currency"/> to update with how much amount and what action</param>
        /// <returns>A <see cref="ServiceResponse"/> object that will tell you if the operation was successful</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item>The total <see cref="CurrencyTrade"/> array will need to leave <see cref="Model.Currency"/> in a correct state. <see cref="Model.Currency.Amount"/> cannot be less than or equal to 0</item>
        /// <item>If one <see cref="CurrencyTrade"/> will leave any <see cref="Model.Currency"/> in a non-correct state, the whole array won't be processed</item>
        /// <item>The specific order of the <see cref="CurrencyTrade"/>[] doesn't matter</item>
        /// <item>A <see cref="ServiceResponse"/>.<see cref="ServiceResponse.Failure"/> will be returned if any <see cref="CurrencyTrade"/>.<see cref="CurrencyTrade.Amount"/> in the passed array is equal to or less than 0</item>
        /// </list>
        /// </remarks>
        public ServiceResponse ProcessCurrencyUpdate(params CurrencyTrade[] trades);
    }
}
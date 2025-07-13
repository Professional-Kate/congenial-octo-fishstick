using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    /// <seealso cref="ProcessCurrencyUpdate"/>
    public interface ICurrencyUpdateMediator
    {
        /// <summary>
        /// Uses a passed <see cref="CurrencyUpdate"/> array, or a singular <see cref="CurrencyUpdate"/> to modify a <see cref="Currency"/> model
        /// </summary>
        /// <param name="updates">An array of <see cref="CurrencyUpdate"/>s will dictate what <see cref="Currency"/> to update with how much amount and what action</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>The total <see cref="CurrencyUpdate"/> array will need to leave <see cref="Currency"/> in a correct state. <see cref="Currency.Amount"/> cannot be less than or equal to 0</item>
        /// <item>If one <see cref="CurrencyUpdate"/> will leave any <see cref="Currency"/> in a non-correct state, the whole array won't be processed</item>
        /// <item>The specific order of the <see cref="CurrencyUpdate"/>[] doesn't matter</item>
        /// <item>A <see cref="ServiceResponse"/>.<see cref="ServiceResponse.Failure"/> will be returned if any <see cref="CurrencyUpdate"/>.<see cref="CurrencyUpdate.Amount"/> in the passed array is equal to or less than 0</item>
        /// </list>
        /// </remarks>
        public void ProcessCurrencyUpdate(IReadOnlyList<CurrencyUpdate> updates);
    }
}
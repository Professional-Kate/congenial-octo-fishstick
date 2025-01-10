using IdelPog.Model;

namespace IdelPog.Service
{
    /// <summary>
    /// See <see cref="ICurrencyService"/> for documentation.
    /// </summary>
    public class CurrencyService : ICurrencyService
    {
        public void AddAmount(Currency currency, int amount)
        {
            int newAmount = currency.Amount + amount;
            currency.SetAmount(newAmount);
        }

        public void RemoveAmount(Currency currency, int amount)
        {
            int newAmount = currency.Amount - amount;
            currency.SetAmount(newAmount);
        }
    }
}
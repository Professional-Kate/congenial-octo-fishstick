namespace IdelPog.Main.Service.Currency
{
    /// <summary>
    /// See <see cref="ICurrencyService"/> for documentation.
    /// </summary>
    public class CurrencyService : ICurrencyService
    {
        public void AddAmount(Structures.Models.Currency currency, int amount)
        {
            int newAmount = currency.Amount + amount;
            currency.SetAmount(newAmount);
        }

        public void RemoveAmount(Structures.Models.Currency currency, int amount)
        {
            int newAmount = currency.Amount - amount;
            currency.SetAmount(newAmount);
        }
    }
}
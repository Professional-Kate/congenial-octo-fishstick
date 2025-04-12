namespace IdelPog.Engine.Service.Currency
{
    /// <summary>
    /// See <see cref="ICurrencyService"/> for documentation.
    /// </summary>
    public class CurrencyService : ICurrencyService
    {
        public void AddAmount(Structures.Currency currency, int amount)
        {
            int newAmount = currency.Amount + amount;
            currency.SetAmount(newAmount);
        }

        public void RemoveAmount(Structures.Currency currency, int amount)
        {
            int newAmount = currency.Amount - amount;
            currency.SetAmount(newAmount);
        }
    }
}
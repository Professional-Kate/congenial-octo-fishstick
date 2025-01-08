namespace IdelPog.Service.Currency
{
    /// <summary>
    /// See <see cref="ICurrencyService"/> for documentation.
    /// </summary>
    public class CurrencyService : ICurrencyService
    {
        public void AddAmount(Model.Currency currency, int amount)
        {
            int newAmount = currency.Amount + amount;
            currency.SetAmount(newAmount);
        }

        public void RemoveAmount(Model.Currency currency, int amount)
        {
            int newAmount = currency.Amount - amount;
            currency.SetAmount(newAmount);
        }
    }
}
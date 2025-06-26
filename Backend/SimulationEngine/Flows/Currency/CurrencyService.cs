namespace IdelPog.SimulationEngine.Flows.Currency
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
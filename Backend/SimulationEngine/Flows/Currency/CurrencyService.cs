using IdelPog.SimulationEngine.Currency.Assertions;

namespace IdelPog.SimulationEngine.Currency
{
    /// <summary>
    /// See <see cref="ICurrencyService"/> for documentation.
    /// </summary>
    public class CurrencyService : ICurrencyService
    {
        private readonly ICurrencyAssertion _currencyAssertion;

        public CurrencyService(ICurrencyAssertion currencyAssertion)
        {
            _currencyAssertion = currencyAssertion;
        }

        public void AddAmount(Models.Currency currency, uint amount)
        {
            uint newAmount = currency.Amount + amount;
            currency.Amount = newAmount;
        }

        public void RemoveAmount(Models.Currency currency, uint amount)
        {
            _currencyAssertion.AssertSufficientCurrency(currency.Amount, amount, currency.CurrencyType);
            uint newAmount = currency.Amount - amount;
            currency.Amount = newAmount;
        }
    }
}
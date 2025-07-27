using IdelPog.SimulationEngine.Currency.Assertions;

namespace IdelPog.SimulationEngine.Currency
{
    /// <summary>
    /// See <see cref="ICurrencyService"/> for documentation.
    /// </summary>
    public class CurrencyService : ICurrencyService
    {
        private readonly INumberAssertion _numberAssertion;
        private readonly ICurrencyAssertion _currencyAssertion;

        public CurrencyService(INumberAssertion numberAssertion, ICurrencyAssertion currencyAssertion)
        {
            _numberAssertion = numberAssertion;
            _currencyAssertion = currencyAssertion;
        }

        public void AddAmount(Currency currency, int amount)
        {
            _numberAssertion.AssertNonNegative(amount);

            int newAmount = currency.Amount + amount;
            currency.SetAmount(newAmount);
        }

        public void RemoveAmount(Currency currency, int amount)
        {
            _numberAssertion.AssertNonNegative(amount);

            _currencyAssertion.AssertSufficientCurrency(currency.Amount, amount, currency.CurrencyType);
            int newAmount = currency.Amount - amount;
            currency.SetAmount(newAmount);
        }
    }
}
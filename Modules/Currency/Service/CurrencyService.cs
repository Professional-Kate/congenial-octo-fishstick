using IdelPog.Currency.Assertion.Interface;
using IdelPog.Currency.Service.Interface;

namespace IdelPog.Currency.Service
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

        public void AddAmount(Contracts.Currency currency, uint amount)
        {
            uint newAmount = currency.Amount + amount;
            currency.Amount = newAmount;
        }

        public void RemoveAmount(Contracts.Currency currency, uint amount)
        {
            _currencyAssertion.AssertSufficientCurrency(currency.Amount, amount, currency.CurrencyType);
            uint newAmount = currency.Amount - amount;
            currency.Amount = newAmount;
        }
    }
}
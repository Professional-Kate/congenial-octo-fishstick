using IdelPog.Common.Commands;
using IdelPog.SimulationEngine.Currency.Assertions;

namespace IdelPog.SimulationEngine.Currency
{
    /// <summary>
    /// See <see cref="ICurrencyService"/> for documentation.
    /// </summary>
    public class CurrencyService : ICurrencyService
    {
        private readonly IAssertPositive _assertPositive;
        private readonly IAssertEnoughCurrency _assertEnoughCurrency;

        public CurrencyService(IAssertPositive assertPositive,  IAssertEnoughCurrency assertEnoughCurrency)
        {
            _assertPositive = assertPositive;
            _assertEnoughCurrency = assertEnoughCurrency;
        }

        public void AddAmount(Currency currency, int amount)
        {
            _assertPositive.AssertNumberIsPositive<CurrencyUpdate>(amount);
            
            int newAmount = currency.Amount + amount;
            currency.SetAmount(newAmount);
        }

        public void RemoveAmount(Currency currency, int amount)
        {
            _assertPositive.AssertNumberIsPositive<CurrencyUpdate>(amount);
            
            _assertEnoughCurrency.Handle(currency.Amount, amount, currency.CurrencyType);
            int newAmount = currency.Amount - amount;
            currency.SetAmount(newAmount);
        }
    }
}
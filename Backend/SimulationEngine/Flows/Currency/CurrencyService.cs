using IdelPog.SimulationEngine.Currency.Assertions;
using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    /// <summary>
    /// See <see cref="ICurrencyService"/> for documentation.
    /// </summary>
    public class CurrencyService : ICurrencyService
    {
        private readonly IAssertPositive _assertPositive;

        public CurrencyService(IAssertPositive assertPositive)
        {
            _assertPositive = assertPositive;
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
            
            int newAmount = currency.Amount - amount;
            // TODO: assert newAmount is positive
            currency.SetAmount(newAmount);
        }
    }
}
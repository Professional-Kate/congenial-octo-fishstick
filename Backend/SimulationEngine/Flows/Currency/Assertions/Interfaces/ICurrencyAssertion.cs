using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public interface ICurrencyAssertion
    {
        public void AssertSufficientCurrency(uint currencyAmount, uint removeAmount, CurrencyType currencyType);
    }
}
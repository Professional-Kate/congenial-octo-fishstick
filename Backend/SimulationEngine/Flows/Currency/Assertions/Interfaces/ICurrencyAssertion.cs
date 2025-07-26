using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public interface ICurrencyAssertion
    {
        public void AssertSufficientCurrency(int currencyAmount, int removeAmount, CurrencyType currencyType);
    }
}
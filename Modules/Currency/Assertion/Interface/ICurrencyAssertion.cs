using IdelPog.Core.Contracts.Enum;

namespace IdelPog.Currency.Assertion.Interface
{
    public interface ICurrencyAssertion
    {
        public void AssertSufficientCurrency(uint currencyAmount, uint removeAmount, CurrencyType currencyType);
    }
}
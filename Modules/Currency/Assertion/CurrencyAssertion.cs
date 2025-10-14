using IdelPog.Core.Contracts.Enum;
using IdelPog.Currency.Assertion.Interface;
using IdelPog.Currency.Exceptions;

namespace IdelPog.Currency.Assertion
{
    public sealed class CurrencyAssertion : ICurrencyAssertion
    {
        public void AssertSufficientCurrency(uint currencyAmount, uint removeAmount, CurrencyType currencyType)
        {
            if (currencyAmount < removeAmount)
            {
                throw new NotEnoughCurrencyException(currencyType, currencyAmount, removeAmount);
            }
        }
    }
}
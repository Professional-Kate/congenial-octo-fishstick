using IdelPog.Core.Contracts.Enum;
using IdelPog.Core.Validation;
using IdelPog.Core.Validation.Handler.Interface;
using IdelPog.Currency.Assertion.Interface;
using IdelPog.Currency.Exceptions;

namespace IdelPog.Currency.Assertion
{
    public class CurrencyAssertion : BaseAssertion, ICurrencyAssertion
    {
        public CurrencyAssertion(IHandler handler) : base(handler)
        {
        }

        public void AssertSufficientCurrency(uint currencyAmount, uint removeAmount, CurrencyType currencyType)
        {
            Assert<NotEnoughCurrencyException>(() =>
            {
                if (currencyAmount < removeAmount)
                {
                    throw new NotEnoughCurrencyException(currencyType, currencyAmount, removeAmount);
                }
            });
        }
    }
}
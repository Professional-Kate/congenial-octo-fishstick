using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.SimulationEngine.Currency.Assertions
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
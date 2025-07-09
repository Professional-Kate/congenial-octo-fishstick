using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public class AssertEnoughCurrency(IHandler handler) : BaseAssertion<NotEnoughCurrencyException>(handler), IAssertEnoughCurrency
    {
        public void Handle(int currencyAmount, int removeAmount, CurrencyType currencyTypeContext)
        {
            Assert(() =>
            {
                if (currencyAmount < removeAmount)
                {
                    throw new NotEnoughCurrencyException(currencyTypeContext, currencyAmount, removeAmount);
                }
            });
        }
    }
}
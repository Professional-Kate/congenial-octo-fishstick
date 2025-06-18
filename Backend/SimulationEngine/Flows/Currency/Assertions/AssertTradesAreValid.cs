using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;
using IdelPog.Validation.Exceptions;

namespace IdelPog.SimulationEngine.Flows.Currency.Assertions
{
    public class AssertTradesAreValid(IHandler handler) : BaseAssertion<NegativeNumberException>(handler)
    {
        public void Handle(IReadOnlyList<CurrencyTrade> trades)
        {
            Assert(() =>
            {
                foreach (CurrencyTrade currencyTrade in trades)
                {
                    if (currencyTrade.Amount <= 0)
                    {
                        throw new NegativeNumberException(currencyTrade.Amount);
                    }
                }
            });
        }
    }
}
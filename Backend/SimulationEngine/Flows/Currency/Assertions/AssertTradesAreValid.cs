using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Exceptions;
using IdelPog.Validation.Assertions;
using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public class AssertTradesAreValid(IHandler handler) : BaseAssertion<NegativeNumberException>(handler), IAssertTradesAreValid
    {
        public void Handle(IReadOnlyList<CurrencyUpdate> trades)
        {
            Assert(() =>
            {
                foreach (CurrencyUpdate currencyTrade in trades)
                {
                    if (currencyTrade.Amount <= 0)
                    {
                        throw new NegativeNumberException(typeof(CurrencyUpdate));
                    }
                }
            });
        }
    }
}
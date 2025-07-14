using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public interface IAssertTradesAreValid
    {
        public void Handle(IReadOnlyList<CurrencyUpdate> trades);
    }
}
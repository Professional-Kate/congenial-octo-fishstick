using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public interface IAssertTradesAreValid
    {
        public void Handle(IReadOnlyList<CurrencyUpdate> trades);
    }
}
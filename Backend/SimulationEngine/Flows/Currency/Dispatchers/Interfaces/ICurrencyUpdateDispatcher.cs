using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency.Dispatchers
{
    public interface ICurrencyUpdateDispatcher
    {
        public void Dispatch(IReadOnlyList<CurrencyUpdate> trades);
    }
}
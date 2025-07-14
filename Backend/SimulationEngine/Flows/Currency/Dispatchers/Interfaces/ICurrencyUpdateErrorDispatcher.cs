using IdelPog.Common.Enums;

namespace IdelPog.SimulationEngine.Currency.Dispatchers
{
    public interface ICurrencyUpdateErrorDispatcher
    {
        public void Dispatch(IReadOnlyList<CurrencyUpdate> updates, Exception exception);
    }
}
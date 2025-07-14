using IdelPog.Common.Enums;
using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Dispatchers
{
    public interface ICurrencyUpdateErrorDispatcher
    {
        public void Dispatch(IReadOnlyList<CurrencyUpdate> updates, Exception exception);
    }
}
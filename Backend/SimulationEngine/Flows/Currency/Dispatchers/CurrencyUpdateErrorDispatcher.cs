using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Dispatchers
{
    public class CurrencyUpdateErrorDispatcher : ICurrencyUpdateErrorDispatcher
    {
        public void Dispatch(IReadOnlyList<CurrencyUpdate> updates, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
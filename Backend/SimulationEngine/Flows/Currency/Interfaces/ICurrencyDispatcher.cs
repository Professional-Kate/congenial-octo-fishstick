namespace IdelPog.SimulationEngine.Flows.Currency
{
    public interface ICurrencyDispatcher
    {
        public void Dispatch(CurrencyTrade trade);
    }
}
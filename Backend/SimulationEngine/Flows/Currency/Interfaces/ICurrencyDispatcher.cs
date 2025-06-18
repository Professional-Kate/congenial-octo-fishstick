namespace IdelPog.SimulationEngine.Flows.Currency
{
    public interface ICurrencyDispatcher
    {
        public void Dispatch(IReadOnlyList<CurrencyTrade> trades);
    }
}
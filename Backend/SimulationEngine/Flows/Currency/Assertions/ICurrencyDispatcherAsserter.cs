namespace IdelPog.SimulationEngine.Flows.Currency.Assertions
{
    public interface ICurrencyDispatcherAsserter
    {
        public void AssertTradeCollection(IReadOnlyList<CurrencyTrade> trades);
    }
}
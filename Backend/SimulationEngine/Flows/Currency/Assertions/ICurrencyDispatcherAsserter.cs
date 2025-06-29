namespace IdelPog.SimulationEngine.Currency.Assertions
{
    public interface ICurrencyDispatcherAsserter
    {
        public void AssertTradeCollection(IReadOnlyList<CurrencyTrade> trades);
    }
}
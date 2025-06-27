namespace IdelPog.SimulationEngine.Flows.Currency
{
    public interface ICurrencyUpdateFactory
    {
        public CurrencyUpdateDTO[] CreateFrom(IReadOnlyList<CurrencyTrade> trades);
    }
}
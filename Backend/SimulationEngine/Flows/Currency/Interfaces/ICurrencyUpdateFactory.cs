namespace IdelPog.SimulationEngine.Currency
{
    public interface ICurrencyUpdateFactory
    {
        public CurrencyUpdateDTO[] CreateFrom(IReadOnlyList<CurrencyTrade> trades);
    }
}
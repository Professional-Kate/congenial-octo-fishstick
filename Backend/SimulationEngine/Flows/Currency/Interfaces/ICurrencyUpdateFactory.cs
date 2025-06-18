namespace IdelPog.SimulationEngine.Flows.Currency
{
    public interface ICurrencyUpdateFactory
    {
        public IReadOnlyList<CurrencyUpdateDTO> CreateFrom(IReadOnlyList<CurrencyTrade> trades);
    }
}
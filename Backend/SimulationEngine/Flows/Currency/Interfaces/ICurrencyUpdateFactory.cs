namespace IdelPog.SimulationEngine.Flows.Currency
{
    public interface ICurrencyUpdateFactory
    {
        public CurrencyUpdateDTO CreateFrom(CurrencyTrade currencyTrade);
    }
}
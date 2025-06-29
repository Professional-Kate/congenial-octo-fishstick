namespace IdelPog.SimulationEngine.Currency
{
    public interface ICurrencyUpdateDispatcher
    {
        public void Dispatch(CurrencyUpdateDTO[] updates);
    }
}
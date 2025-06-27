namespace IdelPog.SimulationEngine.Flows.Currency
{
    public interface ICurrencyUpdateDispatcher
    {
        public void Dispatch(CurrencyUpdateDTO[] updates);
    }
}
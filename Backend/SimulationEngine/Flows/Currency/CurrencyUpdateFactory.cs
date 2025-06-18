namespace IdelPog.SimulationEngine.Flows.Currency
{
    public class CurrencyUpdateFactory : ICurrencyUpdateFactory
    {
        public CurrencyUpdateDTO CreateFrom(CurrencyTrade source)
        {
            return new CurrencyUpdateDTO
            {
                Amount = source.Amount,
                Currency = source.Currency,
                Action = source.Action
            };
        }
    }
}
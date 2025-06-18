namespace IdelPog.SimulationEngine.Flows.Currency
{
    public class CurrencyUpdateFactory : ICurrencyUpdateFactory
    {
        public IReadOnlyList<CurrencyUpdateDTO> CreateFrom(IReadOnlyList<CurrencyTrade> trades)
        {
            List<CurrencyUpdateDTO> result = [];

            foreach (CurrencyTrade currencyTrade in trades)
            {
                result.Add(new CurrencyUpdateDTO
                {
                    Action = currencyTrade.Action,
                    Currency = currencyTrade.Currency,
                    Amount = currencyTrade.Amount,
                });
            }
            
            return result;
        }
    }
}
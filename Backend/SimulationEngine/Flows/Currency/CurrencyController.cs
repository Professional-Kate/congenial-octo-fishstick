using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyController(ICurrencyUpdateMediator currencyUpdateMediator) : ICurrencyController
    {
        public void UpdateCurrency(IReadOnlyList<CurrencyTrade> trades)
        {
            currencyUpdateMediator.ProcessCurrencyUpdate(trades);
        }

        public void CreateCurrency(IReadOnlyList<CurrencyCreation> commands)
        {
            throw new NotImplementedException();
        }
    }
} 
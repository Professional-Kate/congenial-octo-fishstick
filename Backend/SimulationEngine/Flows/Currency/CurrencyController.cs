using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyController(ICurrencyMediator currencyMediator) : ICurrencyController
    {
        public void UpdateCurrency(IReadOnlyList<CurrencyTrade> trades)
        {
            currencyMediator.ProcessCurrencyUpdate(trades);
        }

        public void CreateCurrency(IReadOnlyList<CurrencyCreation> commands)
        {
            throw new NotImplementedException();
        }
    }
} 
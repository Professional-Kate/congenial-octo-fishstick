using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public class CurrencyController(ICurrencyUpdateMediator currencyUpdateMediator, ICurrencyCreationMediator currencyCreationMediator) : ICurrencyController
    {
        public void UpdateCurrency(IReadOnlyList<CurrencyTrade> trades)
        {
            currencyUpdateMediator.ProcessCurrencyUpdate(trades);
        }

        public void CreateCurrency(IReadOnlyList<CurrencyCreation> commands)
        {
            currencyCreationMediator.CreateCurrency(commands);
        }
    }
} 
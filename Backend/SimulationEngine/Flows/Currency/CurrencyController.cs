using IdelPog.Common.Commands;
using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    /// <inheritdoc cref="ICurrencyController"/>
    public class CurrencyController(ICurrencyUpdateMediator currencyUpdateMediator, ICurrencyCreationMediator currencyCreationMediator) : ICurrencyController
    {
        public void UpdateCurrency(IReadOnlyList<CurrencyUpdate> trades)
        {
            currencyUpdateMediator.ProcessCurrencyUpdate(trades);
        }

        public void CreateCurrency(IReadOnlyList<CurrencyCreation> commands)
        {
            currencyCreationMediator.CreateCurrency(commands);
        }
    }
}
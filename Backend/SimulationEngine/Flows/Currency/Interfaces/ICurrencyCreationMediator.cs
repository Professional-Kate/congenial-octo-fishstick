using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public interface ICurrencyCreationMediator
    {
        public void CreateCurrency(IReadOnlyList<CurrencyCreation> currencies);
    }
}
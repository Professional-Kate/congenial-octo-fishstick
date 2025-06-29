using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyCreationErrorFactory : ICurrencyCreationErrorFactory
    {
        public void CreateCurrencyCreationError(IReadOnlyList<CurrencyCreation> currencyCreations, Exception exception)
        {
            throw new NotImplementedException();
        }
    }
}
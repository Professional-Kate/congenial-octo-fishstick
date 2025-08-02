using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Responses;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public interface ICurrencyCreationResponseFactory
    {
        public CurrencyCreationResponse CreateFrom(IReadOnlyList<CurrencyCreation> currencyCreations);

        public CurrencyCreationResponse CreateFrom(CurrencyCreation currencyCreation);
    }
}
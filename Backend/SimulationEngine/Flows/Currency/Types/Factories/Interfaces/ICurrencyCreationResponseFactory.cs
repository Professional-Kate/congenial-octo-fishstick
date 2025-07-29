using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Responses;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public interface ICurrencyCreationResponseFactory
    {
        public CurrencyCreationResponse[] CreateFrom(IReadOnlyList<CurrencyCreation> trades);

        public CurrencyCreationResponse CreateFrom(CurrencyCreation trade);
    }
}
using IdelPog.Common.Commands;
using IdelPog.SimulationEngine.Currency.Responses;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public interface ICurrencyUpdateResponseFactory
    {
        public CurrencyUpdateResponse[] CreateFrom(IReadOnlyList<CurrencyUpdate> trades);
        
        public CurrencyUpdateResponse CreateFrom(CurrencyUpdate trade);
    }
}
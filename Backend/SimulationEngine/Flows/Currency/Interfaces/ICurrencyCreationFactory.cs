using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency
{
    public interface ICurrencyCreationFactory
    {
        public CurrencyCreationDTO[] CreateFrom(IReadOnlyList<CurrencyCreation> trades);
    }
}
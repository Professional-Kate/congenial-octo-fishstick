using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public interface ICurrencyCreationDTOFactory
    {
        public CurrencyCreationDTO[] CreateFrom(IReadOnlyList<CurrencyCreation> trades);

        public CurrencyCreationDTO CreateFrom(CurrencyCreation trade);
    }
}
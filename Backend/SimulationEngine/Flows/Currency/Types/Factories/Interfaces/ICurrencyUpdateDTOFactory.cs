using IdelPog.Common.Commands;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public interface ICurrencyUpdateDTOFactory
    {
        public CurrencyUpdateDTO[] CreateFrom(IReadOnlyList<CurrencyUpdate> trades);
    }
}
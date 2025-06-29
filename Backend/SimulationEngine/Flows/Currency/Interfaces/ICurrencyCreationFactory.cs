using IdelPog.SimulationEngine.Currency.Commands;

namespace IdelPog.SimulationEngine.Currency
{
    public interface ICurrencyCreationFactory
    {
        public CurrencyCreationDTO[] CreateFrom(IReadOnlyList<CurrencyCreation> trades);
    }
}
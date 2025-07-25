using IdelPog.Common.DTO;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public interface IErrorFactory
    {
        public ErrorDTO CreateError(Exception exception);
    }
}
using IdelPog.Common.DTO;
using IdelPog.SimulationEngine.Currency.DTO;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class ErrorFactory : IErrorFactory
    {
        public ErrorDTO CreateError(Exception exception)
        {
            return new ErrorDTO
            {
                Exception = exception
            };
        }
    }
}
using IdelPog.Common.Commands;
using IdelPog.Common.Errors;
using IdelPog.Common.Factories;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyUpdateErrorFactory : IErrorFactory<CurrencyUpdateError, IReadOnlyList<CurrencyUpdate>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public CurrencyUpdateErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public CurrencyUpdateError Create<TException>(IReadOnlyList<CurrencyUpdate> context, TException exception) where TException : Exception
        {
            return new CurrencyUpdateError
            {
                CurrencyUpdates = context.ToArray(),
                BaseErrorDetails = _baseErrorFactory.Create(exception)
            };
        }
    }
}
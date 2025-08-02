using IdelPog.Common.Commands;
using IdelPog.Common.Errors;
using IdelPog.Common.Factories;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyCreationErrorFactory: IErrorFactory<CurrencyCreationError, IReadOnlyList<CurrencyCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public CurrencyCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public CurrencyCreationError Create<TException>(IReadOnlyList<CurrencyCreation> context, TException exception) where TException : Exception
        {
            return new CurrencyCreationError
            {
                CurrencyCreations = context.ToArray(),
                BaseErrorDetails = _baseErrorFactory.Create(exception)
            };
        }
    }
}
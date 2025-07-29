using IdelPog.Common.Factories;
using IdelPog.SimulationEngine.Currency.Commands;
using IdelPog.SimulationEngine.Currency.Responses;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyCreationErrorFactory: IErrorFactory<CurrencyCreationError, IReadOnlyList<CurrencyCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;
        private readonly ICurrencyCreationResponseFactory _currencyCreationResponseFactory;

        public CurrencyCreationErrorFactory(IBaseErrorFactory baseErrorFactory, ICurrencyCreationResponseFactory currencyCreationResponseFactory)
        {
            _baseErrorFactory = baseErrorFactory;
            _currencyCreationResponseFactory = currencyCreationResponseFactory;
        }

        public CurrencyCreationError Create<TException>(IReadOnlyList<CurrencyCreation> context, TException exception) where TException : Exception
        {
            return new CurrencyCreationError
            {
                CurrencyCreations = _currencyCreationResponseFactory.CreateFrom(context),
                BaseErrorDetails = _baseErrorFactory.Create(exception)
            };
        }
    }
}
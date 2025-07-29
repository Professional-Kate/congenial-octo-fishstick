using IdelPog.Common.Commands;
using IdelPog.Common.Factories;
using IdelPog.SimulationEngine.Currency.Responses;

namespace IdelPog.SimulationEngine.Currency.Factories
{
    public class CurrencyUpdateErrorFactory : IErrorFactory<CurrencyUpdateError, IReadOnlyList<CurrencyUpdate>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;
        private readonly ICurrencyUpdateResponseFactory _currencyUpdateResponseFactory;

        public CurrencyUpdateErrorFactory(IBaseErrorFactory baseErrorFactory, ICurrencyUpdateResponseFactory currencyUpdateResponseFactory)
        {
            _baseErrorFactory = baseErrorFactory;
            _currencyUpdateResponseFactory = currencyUpdateResponseFactory;
        }

        public CurrencyUpdateError Create<TException>(IReadOnlyList<CurrencyUpdate> context, TException exception) where TException : Exception
        {
            return new CurrencyUpdateError
            {
                CurrencyUpdates = _currencyUpdateResponseFactory.CreateFrom(context),
                BaseErrorDetails = _baseErrorFactory.Create(exception)
            };
        }
    }
}
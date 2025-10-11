using IdelPog.Core.Contracts.Command;
using IdelPog.Core.Factory.Interface;
using IdelPog.Currency.Contracts.Error;

namespace IdelPog.Currency.Factory
{
    public class CurrencyUpdateErrorFactory : IErrorFactory<CurrencyUpdateError, IReadOnlyList<CurrencyUpdate>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public CurrencyUpdateErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public CurrencyUpdateError Create<TException>(TException exception, IReadOnlyList<CurrencyUpdate> context) where TException : Exception
        {
            return new CurrencyUpdateError
            {
                CurrencyUpdates = context.ToArray(),
                BaseErrorDetails = _baseErrorFactory.Create(exception)
            };
        }
    }
}
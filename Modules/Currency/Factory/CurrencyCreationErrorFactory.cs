using IdelPog.Core.Factory.Interface;
using IdelPog.Currency.Contracts.Command;
using IdelPog.Currency.Contracts.Error;

namespace IdelPog.Currency.Factory
{
    public class CurrencyCreationErrorFactory: IErrorFactory<CurrencyCreationError, IReadOnlyList<CurrencyCreation>>
    {
        private readonly IBaseErrorFactory _baseErrorFactory;

        public CurrencyCreationErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public CurrencyCreationError Create<TException>(TException exception, IReadOnlyList<CurrencyCreation> context) where TException : Exception
        {
            return new CurrencyCreationError
            {
                CurrencyCreations = context.ToArray(),
                BaseErrorDetails = _baseErrorFactory.Create(exception)
            };
        }
    }
}
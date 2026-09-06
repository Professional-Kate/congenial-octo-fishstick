using IdelPog.Core.Contracts;
using IdelPog.Core.Factory.Interface;

namespace IdelPog.Core.Factory
{
    public sealed class BufferedErrorFactory<T> : IErrorFactory<BufferedError<T>, IReadOnlyList<T>>  {
        
        private readonly IBaseErrorFactory _baseErrorFactory;

        public BufferedErrorFactory(IBaseErrorFactory baseErrorFactory)
        {
            _baseErrorFactory = baseErrorFactory;
        }

        public BufferedError<T> Create<TException>(TException exception, IReadOnlyList<T> context) where TException : Exception
        {
            return new BufferedError<T>
            {
                Commands = context.ToArray(),
                BaseError = _baseErrorFactory.Create(exception)
            };
        }
    }
}
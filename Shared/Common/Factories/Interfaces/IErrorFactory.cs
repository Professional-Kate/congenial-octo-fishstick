namespace IdelPog.Common.Factories
{
    public interface IErrorFactory<out TError, in TContext> : IErrorFactory
    {
        public TError Create<TException>(TContext context, TException exception) where TException : Exception;
    }

    public interface IErrorFactory;
}
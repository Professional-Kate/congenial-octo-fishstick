namespace IdelPog.Core.Factory.Interface
{
    public interface IErrorFactory<out TError, in TContext>
    {
        public TError Create<TException>(TException exception, TContext context) where TException : Exception;
    }
}
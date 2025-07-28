namespace IdelPog.Common.Factories
{
    public interface IErrorFactory<out TErrorDTO, in TContext>
    {
        public TErrorDTO Create<TException>(TContext context, TException exception) where TException : Exception;
    }
}
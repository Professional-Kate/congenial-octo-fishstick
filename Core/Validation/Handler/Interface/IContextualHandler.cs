namespace IdelPog.Core.Validation.Handler.Interface
{
    public interface IContextualHandler<in TContext>
    {
        public void Handle<TException>(TException exception, TContext context) where TException : Exception;
    }
}
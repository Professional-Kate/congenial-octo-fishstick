namespace IdelPog.Validation.Assertions.Handlers.Interfaces
{
    public interface IContextualHandler<in TContext>
    {
        public void Handle<TException>(TException exception, TContext context) where TException : Exception;
    }
}
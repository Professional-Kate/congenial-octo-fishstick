namespace IdelPog.Validation.Assertions.Handlers.Interfaces
{
    public interface IHandler
    {
        public void Handle<TException>(TException exception) where TException : Exception;
    }
}
namespace IdelPog.Core.Validation.Handler.Interface
{
    public interface IHandler
    {
        public void Handle<TException>(TException exception) where TException : Exception;
    }
}
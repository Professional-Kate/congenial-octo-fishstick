using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Validation.Handler
{
    /// <summary>
    /// This handler will throw any passed exception
    /// </summary>
    public class ThrowHandler : IHandler
    {
        public void Handle<TException>(TException exception) where TException : Exception
        {
            throw exception;
        }
    }
}
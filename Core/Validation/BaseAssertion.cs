using IdelPog.Core.Validation.Handler.Interface;

namespace IdelPog.Core.Validation
{
    /// <summary>
    /// Base assertion class. This class contains a method for handling thrown exceptions using a passed <see cref="IHandler"/>
    /// </summary>
    public abstract class BaseAssertion
    {
        private readonly IHandler _handler;
        
        protected BaseAssertion(IHandler handler)
        {
            _handler = handler;
        }

        protected void Assert<TException>(Action action) where TException : Exception
        {
            try
            {
                action();
            }
            catch (TException exception)
            {
                _handler.Handle(exception);
            }
        }
    }
}
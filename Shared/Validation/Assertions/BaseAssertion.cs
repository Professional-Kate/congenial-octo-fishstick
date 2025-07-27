using IdelPog.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Validation.Assertions
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

        /// <summary>
        /// Executes the passed action, automatically handling the thrown exception
        /// </summary>
        /// <param name="action">The assertion code. This should contain a throw statement</param>
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
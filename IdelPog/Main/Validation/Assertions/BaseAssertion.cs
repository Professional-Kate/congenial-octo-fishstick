using IdelPog.Main.Validation.Assertions.Handlers.Interfaces;

namespace IdelPog.Main.Validation.Assertions
{
    /// <summary>
    /// Base assertion class. This class contains a method for handling thrown exceptions using a passed <see cref="IHandler"/>
    /// </summary>
    /// <typeparam name="T">The type of the <see cref="Exception"/> to be handled</typeparam>
    /// <seealso cref="Assert"/>
    /// <seealso cref="IHandler"/>
    public abstract class BaseAssertion<T> where T: Exception
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
        protected void Assert(Action action)
        {
            try
            {
                action();
            }
            catch (T exception)
            {
                _handler.Handle(exception);
            }
        }
    }
}